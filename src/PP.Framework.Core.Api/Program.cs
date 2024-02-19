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
// FeatureFactory.Features.Add("Feature", typeof(Feature));
// FeatureFactory.Events.Add("Test", typeof(EventFeatureTest));
#endregion
app.UseMiddleware<FeatureHealthMiddleware>();
app.UseMiddleware<FeatureSecurityMiddleware>();
app.UseMiddleware<FeaterCoreMiddleware>();
// Run App
app.Run();