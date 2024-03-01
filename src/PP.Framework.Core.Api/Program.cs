using PaulPhillips.Framework.Feature.Core;
using PaulPhillips.Framework.Feature.Helpers;
using PaulPhillips.Framework.Feature.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.RegisterFeatureAll();

var app = builder.Build();

#region IOC Setup
IoC.Build();
#endregion

#region Feature Registration
//FeatureFactory.Features.Add("Feature", typeof(Feature));

#endregion

app.UseFeatureFramework();
// Run App
app.Run();