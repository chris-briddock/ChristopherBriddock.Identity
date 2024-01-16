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
    /// Exposes an endpoint to allow the user to update their phone number.
    /// </summary>
    public class UpdatePhoneNumberEndpoint : EndpointBaseAsync
                                       .WithRequest<UpdatePhoneNumberRequest>
                                       .WithActionResult
    {
        /// <summary>
        /// The application service provider.
        /// </summary>
        public IServiceProvider Services { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="UpdatePhoneNumberEndpoint"/>
        /// </summary>
        /// <param name="services"></param>
        public UpdatePhoneNumberEndpoint(IServiceProvider services)
        {
            Services = services;
        }

        /// <summary>
        /// Allows a user to update their email address.
        /// </summary>
        /// <param name="request">The object which encapsulates the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A new <see cref="ActionResult"/></returns>
        [HttpPost("/account/phonenumber")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public override async Task<ActionResult> HandleAsync(UpdatePhoneNumberRequest request, CancellationToken cancellationToken = default)
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

                var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);

                var result = await userManager.ChangePhoneNumberAsync(user, request.PhoneNumber, token);
            
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
