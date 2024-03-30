using PaulPhillips.Feature.BasicExample.Features.FeatureDemo;
using PaulPhillips.Framework.Feature.Core;
using PaulPhillips.Framework.Feature.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterFeatureAll();

var app = builder.Build();

FeatureFactory.Features.Add("CreateCustomer", typeof(CreateCustomerFeature));

app.UseFeatureFramework();

app.Run();