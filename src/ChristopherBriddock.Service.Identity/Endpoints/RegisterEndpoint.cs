﻿using ChristopherBriddock.ApiEndpoints;
using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Common.Messaging;
using ChristopherBriddock.Service.Identity.Constants;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using ChristopherBriddock.Service.Identity.Providers;
using ChristopherBriddock.Service.Identity.Publishers;
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
/// <param name="logger">The application's logger.</param>
public sealed class RegisterEndpoint(IServiceProvider services,
                                    ILogger<RegisterEndpoint> logger) : EndpointBaseAsync
                                                                        .WithRequest<RegisterRequest>
                                                                        .WithoutParam
                                                                        .WithActionResult
{
    /// <inheritdoc/>
    public IServiceProvider Services { get; } = services;
    /// <inheritdoc/>
    public ILogger<RegisterEndpoint> Logger { get; } = logger;

    /// <summary>
    /// Allows a user to register a new user.
    /// </summary>
    /// <param name="request">The object which encapsulates the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A new <see cref="ActionResult"/></returns>
    [HttpPost("/register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> HandleAsync(RegisterRequest request,
                                                         CancellationToken cancellationToken = default)
    {
        try
        {
            var userManager = Services.GetService<UserManager<ApplicationUser>>()!;
            var roleManager = Services.GetService<RoleManager<ApplicationRole>>()!;
            IEmailPublisher emailPublisher = Services.GetService<IEmailPublisher>()!;
            var linkProvider = Services.GetService<ILinkProvider>()!;
            var httpContext = Services.GetService<IHttpContextAccessor>()!;

            var existingUser = await userManager.FindByEmailAsync(request.EmailAddress);

            if (existingUser is not null && !existingUser.IsDeleted)
            {
                return StatusCode(StatusCodes.Status409Conflict, "User is deleted, or already exists.");
            }

            ApplicationUser user = new()
            {
                Email = request.EmailAddress,
                PhoneNumber = request.PhoneNumber
            };
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, $"""{request.Password}""");
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

            RouteValueDictionary routeValues = new()
            {
                ["EmailAddress"] = user.Email,
                ["Code"] = code
            };

           var link =  linkProvider.GetUri(httpContext.HttpContext!, "confirmemail", routeValues);

            EmailMessage message = new()
            {
                EmailAddress = user.Email!,
                Link = link,
                Type = EmailPublisherConstants.Register
            };
            await emailPublisher.Publish(message, cancellationToken);

            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            Logger.LogError("Error in endpoint: {endpointName} - {methodName} Error details: {ex}", nameof(RegisterEndpoint), nameof(HandleAsync), ex);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
