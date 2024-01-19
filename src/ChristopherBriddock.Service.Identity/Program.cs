using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwagger("ChristopherBriddock.Service.Identity.xml");
builder.Services.AddCustomAuthentication();
builder.Services.AddCustomAuthorization();
builder.Services.AddAuthorizationBuilder();
builder.Services.AddIdentity();
builder.Services.AddApplicationServices();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("Default")!);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseSession();
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCustomHealthCheckMapping();
await app.UseDatabaseMigrationsAsync<AppDbContext>();
await app.RunAsync();
