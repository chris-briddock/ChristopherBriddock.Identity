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
    /// Exposes an endpoint to allow the user to update their password.
    /// </summary>
    public class UpdatePasswordEndpoint : EndpointBaseAsync
                                          .WithRequest<UpdatePasswordRequest>
                                          .WithActionResult
    {
        /// <summary>
        /// The application service provider.
        /// </summary>
        public IServiceProvider Services { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="UpdatePasswordEndpoint"/>
        /// </summary>
        /// <param name="services"></param>
        public UpdatePasswordEndpoint(IServiceProvider services)
        {
            Services = services;
        }

        /// <summary>
        /// Allows a user to update their password.
        /// </summary>
        /// <param name="request">The object which encapsulates the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A new <see cref="ActionResult"/></returns>
        [HttpPost("/account/password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<ActionResult> HandleAsync(UpdatePasswordRequest request, CancellationToken cancellationToken = default)
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
                var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            
                if (!result.Succeeded)
                {
                    return BadRequest();
                }

                return NoContent();
            }
            catch(Exception ex)
            {
                // TODO: Add logging.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
