using ChristopherBriddock.AspNetCore.Extensions;
using ChristopherBriddock.Service.Identity.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ChristopherBriddock.Service.Identity.Options
{
    /// <summary>
    /// Configures JWT Bearer authentication options using the Options pattern.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ConfigureJwtBearerOptions"/> class.
    /// </remarks>
    /// <param name="configuration">The application configuration.</param>
    public sealed class ConfigureJwtBearerOptions(IConfiguration configuration) : IConfigureOptions<JwtBearerOptions>
    {
        /// <summary>
        /// The application configuration.
        /// </summary>
        private IConfiguration Configuration { get; } = configuration;

        /// <summary>
        /// Configures JWT Bearer options.
        /// </summary>
        /// <param name="options">The JWT Bearer authentication options to configure.</param>
        public void Configure(JwtBearerOptions options)
        {
            // Retrieve the JWT secret from the application configuration
            string issuer = Configuration.GetRequiredValueOrThrow("Jwt:Issuer");
            string audience = Configuration.GetRequiredValueOrThrow("Jwt:Audience");
            string jwtSecret = Configuration.GetRequiredValueOrThrow("Jwt:Secret");

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
        }
    }
}
