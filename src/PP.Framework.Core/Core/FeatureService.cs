using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OpenTracing;
using PaulPhillips.Framework.Feature.Commands.Contracts;
using PaulPhillips.Framework.Feature.Core.Contracts;
using PaulPhillips.Framework.Feature.Core.Models;
using PaulPhillips.Framework.Feature.Events.Contracts;
using PaulPhillips.Framework.Feature.Idempotency.Contracts;
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
            ArgumentNullException.ThrowIfNull(context, nameof(context));
            ArgumentNullException.ThrowIfNull(tracer, nameof(tracer));
            ArgumentNullException.ThrowIfNull(eventManager, nameof(eventManager));
            ArgumentNullException.ThrowIfNull(idempotency, nameof(idempotency));

            var responseModel = new ResponseModel();

            context.Response.StatusCode = StatusCodes.Status200OK;

            var featureRequest = new FeatureRequest
            {
                Feature = FeatureFactory.Features.FirstOrDefault(p => $"/{p.Key.ToLower()}"
                                        .Equals(context.Request.Path.ToString(), StringComparison.InvariantCultureIgnoreCase)).Value,

                TraceId = context.Request.Headers["TraceId"],
                IdempotencyKey = context.Request.Headers["IdempotencyKey "]
            };

            var requestSpan = tracer.BuildSpan(context.Request.Path.ToString()).StartActive(true).Span;

            if (featureRequest.TraceId != null)
            {
                requestSpan.SetTag("TraceId", featureRequest.TraceId);
            }

            if (featureRequest.IdempotencyKey != null)
            {
                requestSpan.SetTag("IdempotencyKey", featureRequest.IdempotencyKey);
            }

            var refreshResponse = true;

            if (featureRequest.Feature != null && Activator.CreateInstance(featureRequest.Feature) is IFeatureCore feature)
            {
                featureRequest.IdempotencyKey = context.Request.Headers["IdempotencyKey"].FirstOrDefault();

                feature.EventManager = eventManager;

                var body = "";
                if (context.Request.Method == "POST")
                {
                    body = await GetRequestBodyAsync(context);

                    if (featureRequest.IdempotencyKey != null)
                    {
                        var idempotencyResult = await idempotency.ManageIdempotencyRequest(featureRequest);
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
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                requestSpan.Log("Idempotency detected duplicate request but hash of body is different, returning bad request.");
                                refreshResponse = false;
                                break;
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
                                    requestSpan.SetTag("Status", StatusCodes.Status400BadRequest);
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
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
                                        requestSpan.SetTag("Status", StatusCodes.Status400BadRequest);
                                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
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
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    requestSpan.SetTag("Status", StatusCodes.Status400BadRequest);
                                }

                            }
                        }

                        if (context.Response.StatusCode == StatusCodes.Status200OK)
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
                                loadDataSpan.SetTag("Status", StatusCodes.Status500InternalServerError);
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            }
                            finally
                            {
                                loadDataSpan.Finish();
                            }


                            var processSpan = tracer.BuildSpan("Process").AsChildOf(requestSpan).Start();
                            try
                            {
                                responseModel.Response = await feature.ProcessAsync(processSpan);
                                if (featureRequest.IdempotencyKey != null)
                                {
                                    var response = JsonConvert.SerializeObject(responseModel.Response);
                                    await idempotency.ManageIdempotencyResponse(featureRequest, response);
                                }

                            }
                            catch (Exception ex)
                            {
                                processSpan.Log($"Process failed, {ex.Message}");
                                processSpan.SetTag("Status", StatusCodes.Status500InternalServerError);
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
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
            }
            
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

