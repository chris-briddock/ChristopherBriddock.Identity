using ChristopherBriddock.Service.Common.Extensions;
using ChristopherBriddock.Service.Email.Extensions;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFeatureManagement();

builder.Services.AddConsumerMessaging();
builder.Services.AddSerilogWithConfiguration();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
await app.RunAsync();
