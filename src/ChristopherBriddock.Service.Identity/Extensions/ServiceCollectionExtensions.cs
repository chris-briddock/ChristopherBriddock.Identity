using ChristopherBriddock.Service.Identity.Constants;
using ChristopherBriddock.Service.Identity.Data;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Options;
using ChristopherBriddock.Service.Identity.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.FeatureManagement;

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
        ArgumentNullException.ThrowIfNull(services, nameof(services));

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
        services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
        {
            opt.SignIn.RequireConfirmedPhoneNumber = false;
            opt.SignIn.RequireConfirmedEmail = false;
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
            opt.User.RequireUniqueEmail = true;
            opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager<SignInManager<ApplicationUser>>()
        .AddUserManager<UserManager<ApplicationUser>>()
        .AddRoles<ApplicationRole>()
        .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Extension method for adding authentication services to the IServiceCollection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));
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
                                      .GetRequiredService<IConfiguration>();
        IFeatureManager featureManager = services
                                        .BuildServiceProvider()
                                        .GetRequiredService<IFeatureManager>();
        
        services.AddDistributedMemoryCache();

        if (featureManager.IsEnabledAsync(FeatureFlags.Redis).Result)
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = configuration.GetConnectionString("Redis");
                opt.InstanceName = configuration.GetConnectionString("RedisInstanceName");
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
            opt.Cookie.Name = "AspNetCookie.Session";
            opt.Cookie.Expiration = TimeSpan.FromMinutes(60);
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
        var featureManager = services.BuildServiceProvider().GetRequiredService<IFeatureManager>();
        if (featureManager.IsEnabledAsync(FeatureFlags.ApplicationInsights).Result)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
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
}