using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OpenTracing;
using PaulPhillips.Framework.Feature.Commands.Contracts;
using PaulPhillips.Framework.Feature.Core.Contracts;
using PaulPhillips.Framework.Feature.Events.Contracts;
using PaulPhillips.Framework.Feature.Idempotency.Contracts;
using PaulPhillips.Framework.Feature.Idempotency.Models;
using PaulPhillips.Framework.Feature.Models;
using PaulPhillips.Framework.Feature.Query.Contracts;
using System.Text;


namespace PaulPhillips.Framework.Feature.Core
{
    public class FeatureService
    {
        public static IMapper ConfigureMapping(ICommand feature)
        {
            var mapperConfigExpression = new MapperConfigurationExpression();
            feature.SetupMapping(mapperConfigExpression);

            return new MapperConfiguration(mapperConfigExpression).CreateMapper();
        }

        public static async Task RequestHandler(HttpContext context, ITracer tracer, IEventManager eventManager, IIdempotency idempotency)
        {

            context.Response.StatusCode = FeatureServiceStatus.StatusOK;

            var requestSpan = tracer.BuildSpan(context.Request.Path.ToString()).StartActive(true).Span;

            context.Request.Headers.TryGetValue("TraceID", out var traceId);
            if (traceId.Count > 0)
            {
                requestSpan.SetTag("TraceId", traceId);
            }

            var responseModel = new ResponseModel();           

            var refreshResponse = true;
            var featureType = FeatureFactory.Features.FirstOrDefault(
            p => $"/{p.Key.ToLower()}".Equals(context.Request.Path.ToString(), StringComparison.InvariantCultureIgnoreCase)).Value;


            if (featureType != null && Activator.CreateInstance(featureType) is IFeatureCore feature)
            {
                var messageId = context.Request.Headers["MessageId"].FirstOrDefault();
                IdempotencyModel idempotencyModel = new()
                {
                    Id = messageId ?? "",
                    Name = featureType.Name,
                };

                feature.EventManager = eventManager;




                var body = "";
                if (context.Request.Method == "POST" || context.Request.Method == "PATCH")
                {
                    body = await GetRequestBodyAsync(context);

                    if (messageId != null)
                    {
                        idempotencyModel.MessageHash = body.GetHashCode();
                        var idempotencyResult = await idempotency.ManageIdempotencyRequest(idempotencyModel);
                        context.Response.Headers["IdempotencyStatus"] = idempotencyResult.Status.ToString();
                        switch (idempotencyResult.Status)
                        {
                            case Idempotency.Enums.IdempotencyStatus.UseCachedResponse:
                                requestSpan.Log("Idempotency detected duplicate request, using cache response");
                                responseModel.ValidationResult = new Validation.Models.ValidationResultModel();
                                responseModel.Response = idempotencyResult.Response;
                                refreshResponse = false;
                                break;
                            case Idempotency.Enums.IdempotencyStatus.RequestFoundButHashIsDifferent:
                                context.Response.StatusCode = FeatureServiceStatus.StatusBadRequest;
                                requestSpan.Log("Idempotency detected duplicate request but hash of body is different, returning bad request.");
                                refreshResponse = false;
                                break;
                        }
                    }
                }
                if (refreshResponse)
                {

                    var iocSpan = tracer.BuildSpan("IOC").AsChildOf(requestSpan).Start();
                    feature.LoadIocServices(iocSpan);
                    iocSpan.Finish();

                    if (feature is ICommand command)
                    {
                        command.SetData(body);

                        // Perform Validation
                        responseModel.ValidationResult = command.GetValidation()?.Validate();
                        if (responseModel.ValidationResult != null)
                        {
                            if (!responseModel.ValidationResult.Success)
                            {
                                requestSpan.Log($"Validation Request Failed: {responseModel.ValidationResult.ValidationMessages}");
                                requestSpan.SetTag("Status", FeatureServiceStatus.StatusBadRequest);
                                context.Response.StatusCode = FeatureServiceStatus.StatusBadRequest;
                                command.RequestValidationFailed();
                            }
                            else
                            {
                                var mapper = ConfigureMapping(command);
                                command.MapToEntity(mapper, requestSpan);
                                responseModel.ValidationResult = command.GetEntityValidation()?.Validate();
                                if (responseModel.ValidationResult != null && !responseModel.ValidationResult.Success)
                                {
                                    requestSpan.Log($"Validation Entity Failed: {responseModel.ValidationResult.ValidationMessages}");
                                    requestSpan.SetTag("Status", FeatureServiceStatus.StatusBadRequest);
                                    context.Response.StatusCode = FeatureServiceStatus.StatusBadRequest;
                                    command.EntityValidationFailed();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (feature is IQuery query)
                        {
                            query.SetData(context.Request.Query);

                            responseModel.ValidationResult = query.Validate();

                            if (responseModel.ValidationResult != null && !responseModel.ValidationResult.Success)
                            {
                                requestSpan.Log($"Validation Failed: {responseModel.ValidationResult.ValidationMessages}");
                                context.Response.StatusCode = FeatureServiceStatus.StatusBadRequest;
                                requestSpan.SetTag("Status", FeatureServiceStatus.StatusBadRequest);
                            }

                        }
                    }

                    if (context.Response.StatusCode == FeatureServiceStatus.StatusOK)
                    {
                        var loadDataSpan = tracer.BuildSpan("LoadData").AsChildOf(requestSpan).Start();
                        try
                        {
                            // Inform Feature to load any relevant data
                            await feature.LoadData(requestSpan);
                        }
                        catch (Exception ex)
                        {
                            loadDataSpan.Log($"Failed to load data, {ex.Message}");
                            loadDataSpan.SetTag("Status", FeatureServiceStatus.StatusServerError);
                            context.Response.StatusCode = FeatureServiceStatus.StatusServerError;
                        }
                        finally
                        {
                            loadDataSpan.Finish();
                        }


                        var processSpan = tracer.BuildSpan("Process").AsChildOf(requestSpan).Start();
                        try
                        {
                            responseModel.Response = await feature.ProcessAsync(processSpan);
                            if (messageId != null)
                            {
                                idempotencyModel.Response = JsonConvert.SerializeObject(responseModel.Response);
                                await idempotency.ManageIdempotencyResponse(idempotencyModel);
                            }

                        }
                        catch (Exception ex)
                        {
                            processSpan.Log($"Process failed, {ex.Message}");
                            processSpan.SetTag("Status", FeatureServiceStatus.StatusServerError);
                            context.Response.StatusCode = FeatureServiceStatus.StatusServerError;
                        }
                        finally
                        {
                            processSpan.Finish();
                        }

                    }
                }

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(responseModel));

            }

            requestSpan.Finish();
        }




        public static async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
            {
                return string.Empty;
            }

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

    }
}

