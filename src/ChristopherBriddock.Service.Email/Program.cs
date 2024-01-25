using ChristopherBriddock.Service.Common.Extensions;
using ChristopherBriddock.Service.Email.Extensions;
using Microsoft.FeatureManagement;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddFeatureManagement();
builder.Services.AddConsumerMessaging();
builder.Services.AddSerilogWithConfiguration();
var host = builder.Build();
await host.RunAsync();
