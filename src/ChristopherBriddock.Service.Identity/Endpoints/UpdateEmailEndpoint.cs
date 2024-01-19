using Ardalis.ApiEndpoints;
using ChristopherBriddock.Service.Identity.Models;
using ChristopherBriddock.Service.Identity.Models.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChristopherBriddock.Service.Identity.Endpoints
{
    /// <summary>
    /// Exposes an endpoint to allow the user to update their email.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of <see cref="UpdateEmailEndpoint"/>
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="logger"></param>
    public class UpdateEmailEndpoint(IServiceProvider services,
                               ILogger<UpdateEmailEndpoint> logger) : EndpointBaseAsync
                                                                     .WithRequest<UpdatePhoneNmberRequest>
                                                                     .WithActionResult
    {
        /// <summary>
        /// The application service provider.
        /// </summary>
        public IServiceProvider Services { get; set; } = services;
        /// <summary>
        /// The application's logger.
        /// </summary>
        public ILogger<UpdateEmailEndpoint> Logger { get; } = logger;

        /// <summary>
        /// Allows a user to update their email address.
        /// </summary>
        /// <param name="request">The object which encapsulates the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A new <see cref="ActionResult"/></returns>
        [HttpPost("/account/email")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult> HandleAsync(UpdatePhoneNmberRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var userManager = Services.GetRequiredService<UserManager<ApplicationUser>>();
                string emailAddress = User.FindFirst(ClaimTypes.Email)!.Value;
                
                var user = await userManager.FindByEmailAsync(emailAddress);

                if (user is null)
                {
                    return NotFound();
                }
                // NOTE: could email this token to the user, to make a change link.
                var token = await userManager.GenerateChangeEmailTokenAsync(user, request.EmailAddress);

                var result = await userManager.ChangeEmailAsync(user, request.EmailAddress, token);
            
                if (!result.Succeeded)
                {
                    return BadRequest();
                }

                return NoContent();
            }
            catch(Exception ex)
            {
                Logger.LogError($"Error in endpoint: {nameof(AuthoriseEndpoint)} - {nameof(HandleAsync)} Error details: {ex}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
