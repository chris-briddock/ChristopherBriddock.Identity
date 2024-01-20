﻿using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

namespace ChristopherBriddock.Service.Identity.Extensions;

/// <summary>
/// Extensions methods for the <see cref="IWebHostBuilder"/>
/// </summary>
public static class WebHostBuilderExtensions
{
    /// <summary>
    /// Adds Kestrel server configuration to the web host builder.
    /// </summary>
    /// <param name="webHostBuilder">The IWebHostBuilder instance.</param>
    /// <param name="portNumber"></param>
    public static void AddKestrelConfiguration(this IWebHostBuilder webHostBuilder, int portNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(webHostBuilder));
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(portNumber));

        webHostBuilder.ConfigureKestrel((context, options) =>
        {
            options.ListenAnyIP(portNumber, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                listenOptions.UseHttps();
            });
        });
    }
    /// <summary>
    /// Add serilog with option enabled to read from configuration.
    /// </summary>
    /// <param name="webHostBuilder">The updated <see cref="IHostBuilder"/></param>
    public static void AddSerilog(this IHostBuilder webHostBuilder)
    {     
        webHostBuilder.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
        });
    }
}
