using System.Text;
using System.Threading.RateLimiting;
using ChristopherBriddock.AspNetCore.Extensions;
using Persistence.Contexts;
using Domain.Aggregates.User;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Domain.Constants;
using Domain.Contracts;
using Application.Requests;
using Application.Results;
using Application.Commands;
using ChristopherBriddock.Service.Identity.Application.Commands.Handlers;

namespace Application.Extensions;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ASP.NET Identity configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
        {
            opt.SignIn.RequireConfirmedPhoneNumber = false;
            opt.SignIn.RequireConfirmedEmail = true;
            opt.SignIn.RequireConfirmedAccount = false;
            opt.Lockout.AllowedForNewUsers = false;
            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            opt.Lockout.MaxFailedAccessAttempts = 5;
            opt.Password.RequireDigit = true;
            opt.Password.RequiredLength = 12;
            opt.Password.RequiredUniqueChars = 1;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireNonAlphanumeric = true;
            opt.Password.RequireUppercase = true;
            opt.User.RequireUniqueEmail = false;
            opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        })
        .AddEntityFrameworkStores<AppDbContextWrite>()
        .AddSignInManager<SignInManager<ApplicationUser>>()
        .AddUserManager<UserManager<ApplicationUser>>()
        .AddRoles<ApplicationRole>()
        .AddRoleStore<RoleStore<ApplicationRole, AppDbContextWrite, Guid>>()
        .AddUserStore<UserStore<ApplicationUser, ApplicationRole, AppDbContextWrite, Guid>>()
        .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.AddDbContextPool<AppDbContextWrite>(opt =>
        {
            opt.EnableServiceProviderCaching();
        });

        services.AddDbContextPool<AppDbContextRead>(opt =>
        {
            opt.EnableServiceProviderCaching();
        });

        return services;
    }

    /// <summary>
    /// Extension method for adding authentication services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddBearerAuthentication(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.TryAddScoped<IJsonWebTokenProvider, JsonWebTokenProvider>();
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => 
        {
            // Retrieve the JWT secret from the application configuration
            string issuer = configuration.GetRequiredValueOrThrow("Jwt:Issuer");
            string audience = configuration.GetRequiredValueOrThrow("Jwt:Audience");
            string jwtSecret = configuration.GetRequiredValueOrThrow("Jwt:Secret");

            if (string.IsNullOrWhiteSpace(jwtSecret))
                throw new JwtSecretNullOrEmptyException();

            var key = Encoding.ASCII.GetBytes(jwtSecret);
            // Configure token validation parameters
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Save the token in the authentication context
            options.SaveToken = true;
        });

        return services;
    }
    /// <summary>
    /// Adds custom authorization policy.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAuthorizationPolicy(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("UserRolePolicy", opt =>
            {
                opt.RequireRole(RoleConstants.User);
            });

        return services;
    }

    /// <summary>
    /// Add the required services for in-memory and redis services, if redis is enabled in the feature flags.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddSessionCache(this IServiceCollection services)
    {
        IConfiguration configuration = services
                                      .BuildServiceProvider()
                                      .GetRequiredService<IConfiguration>();
        IFeatureManager featureManager = services
                                        .BuildServiceProvider()
                                        .GetRequiredService<IFeatureManager>();
        
        services.AddDistributedMemoryCache();

        if (featureManager.IsEnabledAsync(FeatureFlagConstants.Redis).Result)
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = configuration.GetConnectionString("Redis");
            });
        }
        return services;
    }
    /// <summary>
    /// Adds session support to the application. 
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAppSession(this IServiceCollection services)
    {
        services.AddSession(opt =>
        {
            opt.IdleTimeout = TimeSpan.FromMinutes(60);
            opt.Cookie.Name = ".AspNetCookie.Session";
            opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            opt.Cookie.IsEssential = true;
        });
        return services;
    }
    /// <summary>
    /// Adds Azure Application Insights, if enabled.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAzureAppInsights(this IServiceCollection services)
    {
        var featureManager = services.BuildServiceProvider()
                                     .GetRequiredService<IFeatureManager>();

        if (featureManager.IsEnabledAsync(FeatureFlagConstants.AzApplicationInsights).Result)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>()!;
            services.AddApplicationInsightsTelemetry(options => options.ConnectionString = configuration.GetRequiredValueOrThrow("ApplicationInsights:InstrumentationKey"));
            services.AddApplicationInsightsKubernetesEnricher();
        }
        return services;
    }
    /// <summary>
    /// Add cross origin policy.
    /// </summary>
    /// <remarks>
    /// This is only enabled in development, by the middleware <see cref="CorsMiddlewareExtensions.UseCors(IApplicationBuilder)"/>
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy(CorsConstants.PolicyName, opt =>
            {
                opt.WithOrigins("http://localhost:3000", "http://localhost:4000", "http://localhost:4200");
                opt.AllowAnyHeader();
                opt.AllowAnyMethod();
                opt.AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// Adds publisher messaging for rabbitmq or azure service bus.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    //public static IServiceCollection AddPublisherMessaging(this IServiceCollection services)
    //{
    //    var configuration = services.BuildServiceProvider()
    //                                .GetRequiredService<IConfiguration>();

    //    var featureManager = services.BuildServiceProvider()
    //                                 .GetRequiredService<IFeatureManager>();

    //    var rabbitMqEnabled = featureManager.IsEnabledAsync(FeatureFlagConstants.RabbitMq).Result;

    //    var azServiceBusEnabled = featureManager.IsEnabledAsync(FeatureFlagConstants.AzServiceBus).Result;

    //    if (azServiceBusEnabled)
    //    {
    //        services.AddMassTransit(mt =>
    //        {
    //            mt.UsingAzureServiceBus((context, config) =>
    //            {
    //                config.Host(configuration["Messaging:AzureServiceBus:ConnectionString"]);
    //                config.ConfigureEndpoints(context);
    //            });
    //        });
    //    }
    //    if (rabbitMqEnabled)
    //    {
    //        services.AddMassTransit(mt =>
    //        {
    //            mt.SetKebabCaseEndpointNameFormatter();

    //            mt.UsingRabbitMq((context, config) =>
    //            {

    //                config.Host(configuration["Messaging:RabbitMQ:Hostname"], "/", r =>
    //                {
    //                    r.Username(configuration["Messaging:RabbitMQ:Username"]!);
    //                    r.Password(configuration["Messaging:RabbitMQ:Password"]!);
    //                });
    //                config.ConfigureEndpoints(context);
    //            });
    //        });
    //    }

    //    if (rabbitMqEnabled || azServiceBusEnabled)
    //    {
    //        services.TryAddTransient<IEmailPublisher, EmailPublisher>();
    //    }
    //    else
    //    {
    //        services.TryAddTransient<IEmailPublisher, NullEmailPublisher>();
    //    }

    //    return services;
    //}

    /// <summary>
    /// Adds fixed window rate limiting. 
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services) 
    {
        services.AddRateLimiter(_ => _
        .AddFixedWindowLimiter(policyName: "fixed", options =>
        {
            options.PermitLimit = 4;
            options.Window = TimeSpan.FromSeconds(12);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 2;
        }));

        return services;
    }
}