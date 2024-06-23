using System.Text;
using System.Threading.RateLimiting;
using ChristopherBriddock.AspNetCore.Extensions;
using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Identity.Constants;
using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Exceptions;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Providers;
using ChristopherBriddock.Service.Identity.Publishers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;

namespace ChristopherBriddock.Service.Identity.Extensions;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ASP.NET Identity configuration.
    /// </summary>
    /// <remarks> 
    /// This method uses AddIdentityCore due to AddIdentity adding an authentication scheme <see cref="IdentityConstants.ApplicationScheme"/>
    /// The authentication scheme causes the application to throw an error "System.InvalidOperationException : Scheme already exists: Identity.Application"
    /// I have manually added the authentication scheme in the extension method <see cref="AddBearerAuthentication"/> due to this incorrectly redirecting users
    /// to /Account/Login, which causes a 404 error.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<IRoleValidator<ApplicationRole>, RoleValidator<ApplicationRole>>();
        services.TryAddScoped<IdentityErrorDescriber>();
        services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<ApplicationUser>>();
        services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<ApplicationUser>>();

        services.AddIdentityCore<ApplicationUser>(opt =>
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
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager<SignInManager<ApplicationUser>>()
        .AddUserManager<UserManager<ApplicationUser>>()
        .AddRoles<ApplicationRole>()
        .AddRoleStore<RoleStore<ApplicationRole, AppDbContext, Guid>>()
        .AddUserStore<UserStore<ApplicationUser, ApplicationRole, AppDbContext, Guid>>()
        .AddDefaultTokenProviders();

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

        services.TryAddSingleton<IJsonWebTokenProvider, JsonWebTokenProvider>();
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
            {
                throw new JwtSecretNullOrEmptyException();
            }

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
        })
        .AddCookie(IdentityConstants.ApplicationScheme, s =>
        {
            s.LoginPath = "/";
            s.LogoutPath = "/";
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
                                      .GetService<IConfiguration>()!;
        IFeatureManager featureManager = services
                                        .BuildServiceProvider()
                                        .GetService<IFeatureManager>()!;
        
        services.AddMemoryCache();
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
                                     .GetService<IFeatureManager>()!;

        if (featureManager.IsEnabledAsync(FeatureFlagConstants.AzApplicationInsights).Result)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>()!;
            services.AddApplicationInsightsTelemetry(options => options.ConnectionString = configuration["ApplicationInsights:InstrumentationKey"]);
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
    public static IServiceCollection AddPublisherMessaging(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider()
                                    .GetService<IConfiguration>()!;

        var featureManager = services.BuildServiceProvider()
                                     .GetService<IFeatureManager>()!;

        var rabbitMqEnabled = featureManager.IsEnabledAsync(FeatureFlagConstants.RabbitMq).Result;

        var azServiceBusEnabled = featureManager.IsEnabledAsync(FeatureFlagConstants.AzServiceBus).Result;

        if (azServiceBusEnabled)
        {
            services.AddMassTransit(mt =>
            {
                mt.UsingAzureServiceBus((context, config) =>
                {
                    config.Host(configuration["Messaging:AzureServiceBus:ConnectionString"]);
                    config.ConfigureEndpoints(context);
                });
            });
        }
        if (rabbitMqEnabled)
        {
            services.AddMassTransit(mt =>
            {
                mt.SetKebabCaseEndpointNameFormatter();

                mt.UsingRabbitMq((context, config) =>
                {

                    config.Host(configuration["Messaging:RabbitMQ:Hostname"], "/", r =>
                    {
                        r.Username(configuration["Messaging:RabbitMQ:Username"]!);
                        r.Password(configuration["Messaging:RabbitMQ:Password"]!);
                    });
                    config.ConfigureEndpoints(context);
                });
            });
        }

        if (rabbitMqEnabled || azServiceBusEnabled)
        {
            services.TryAddTransient<IEmailPublisher, EmailPublisher>();
        }
        else
        {
            services.TryAddTransient<IEmailPublisher, NullEmailPublisher>();
        }

        return services;
    }

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