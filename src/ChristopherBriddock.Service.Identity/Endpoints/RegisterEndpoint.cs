using Ardalis.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Constants;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ChristopherBriddock.Service.Identity.Endpoints;

/// <summary>
/// Exposes an endpoint that allows a user to register.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="RegisterEndpoint"/>
/// </remarks>
/// <param name="services">The application's service provider.</param>
/// <param name="emailSender">The application's email sender.</param>
public sealed class RegisterEndpoint(IServiceProvider services,
                                    IEmailProvider emailSender) : EndpointBaseAsync
                                                                  .WithRequest<RegisterRequest>
                                                                  .WithActionResult
{
    /// <inheritdoc/>
    public IServiceProvider Services { get; } = services;
    /// <inheritdoc/>
    public IEmailProvider EmailSender { get; } = emailSender;

    /// <summary>
    /// Allows a user to register a new user.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(RegisterRequest request,
                                                   CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = Services.GetRequiredService<RoleManager<ApplicationRole>>();

            ApplicationUser user = new()
            {
                Email = request.EmailAddress,
                PhoneNumber = request.PhoneNumber
            };
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.Password);
            await userManager.SetUserNameAsync(user, user.Email);
            await userManager.SetEmailAsync(user, user.Email);
            await userManager.SetPhoneNumberAsync(user, user.PhoneNumber);
            await userManager.CreateAsync(user);

            ApplicationRole applicationRole = new()
            {
                Name = RoleConstants.UserRole,
                NormalizedName = RoleConstants.UserRole.ToUpper()
            };

            if (!await roleManager.RoleExistsAsync(applicationRole.Name))
            {
                await roleManager.CreateAsync(applicationRole);
            }
            if (!await userManager.IsInRoleAsync(user, RoleConstants.UserRole))
            {
                await userManager.AddToRoleAsync(user, RoleConstants.UserRole);
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await EmailSender.SendConfirmationLinkAsync(user, request.EmailAddress, code);

            return Created();


        }
        catch (Exception ex)
        {
            // TODO: Add LOGGING.
            return StatusCode(StatusCodes.Status500InternalServerError);
        }


    }
}
