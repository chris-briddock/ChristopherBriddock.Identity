using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Identity.Constants;
using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Options;
using ChristopherBriddock.Service.Identity.Providers;
using ChristopherBriddock.Service.Identity.Publishers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;

namespace ChristopherBriddock.Service.Identity.Extensions;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Swagger with custom configuration.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="xmlFile"></param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services, string xmlFile)
    {
        services.AddSwaggerGen(opt =>
        {
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            opt.IncludeXmlComments(xmlPath);
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
            });
            opt.UseApiEndpoints();
        });

        return services;
    }

    /// <summary>
    /// Adds the ASP.NET Identity configuration.
    /// </summary>
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
        .AddUserStore<UserStore<ApplicationUser,ApplicationRole,AppDbContext,Guid>>()
        .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddCookie(IdentityConstants.ApplicationScheme, o =>
        {
            o.Cookie.Name = IdentityConstants.ApplicationScheme;
            o.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
            };
        })
        .AddCookie(IdentityConstants.ExternalScheme, o =>
        {
            o.Cookie.Name = IdentityConstants.ExternalScheme;
            o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        })
        .AddCookie(IdentityConstants.TwoFactorRememberMeScheme, o =>
        {
            o.Cookie.Name = IdentityConstants.TwoFactorRememberMeScheme;
            o.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
            };
        })
        .AddCookie(IdentityConstants.TwoFactorUserIdScheme, o =>
        {
            o.Cookie.Name = IdentityConstants.TwoFactorUserIdScheme;
            o.Events = new CookieAuthenticationEvents
            {
                OnRedirectToReturnUrl = _ => Task.CompletedTask
            };
            o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        });

        return services;
    }

    /// <summary>
    /// Extension method for adding authentication services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
    {
        services.TryAddSingleton<IJsonWebTokenProvider, JsonWebTokenProvider>();
        services.TryAddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

        return services;
    }
    /// <summary>
    /// Adds custom authorization policy.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("UserRolePolicy", opt =>
            {
                opt.RequireRole(RoleConstants.UserRole);
            });

        return services;
    }

    /// <summary>
    /// Add the required services for in-memory and redis services, if redis is enabled in the feature flags.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        IConfiguration configuration = services
                                      .BuildServiceProvider()
                                      .GetService<IConfiguration>()!;
        IFeatureManager featureManager = services
                                        .BuildServiceProvider()
                                        .GetService<IFeatureManager>()!;

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
    public static IServiceCollection AddCustomSession(this IServiceCollection services)
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
    public static IServiceCollection AddCrossOriginPolicy(this IServiceCollection services)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy(CorsConstants.PolicyName, opt =>
            {
                opt.AllowAnyOrigin();
                opt.AllowAnyHeader();
                opt.AllowAnyMethod();
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
                        r.Username(configuration["Messaging:RabbitMQ:Username"]);
                        r.Password(configuration["Messaging:RabbitMQ:Password"]);
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
}