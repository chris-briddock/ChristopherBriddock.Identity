using ChristopherBriddock.WorkerService.Email;
using Microsoft.FeatureManagement;
using ChristopherBriddock.Service.Common.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddFeatureManagement();
builder.Services.AddConsumerMessaging();
builder.Services.AddSerilogWithConfiguration();


var host = builder.Build();
host.Run();
