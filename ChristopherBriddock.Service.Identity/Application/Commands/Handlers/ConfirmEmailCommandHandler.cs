using Application.Commands;
using Application.Requests;
using Application.Results;
using Domain.Aggregates.User;
using Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace ChristopherBriddock.Service.Identity.Application.Commands.Handlers;

/// <summary>
/// Represents a command to confirm a user's email address.
/// </summary>
public sealed class ConfirmEmailCommandHandler : IAsyncCommandHandler<ConfirmEmailCommand, ConfirmEmailResult>
{
    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfirmEmailCommandHandler"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public ConfirmEmailCommandHandler(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// Executes the confirm email command asynchronously.
    /// </summary>
    /// <param name="request">The request containing email address and confirmation code.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the confirmation email result.</returns>
    public async Task<ConfirmEmailResult> HandleAsync(ConfirmEmailCommand request)
    {
        try
        {
            var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                IdentityError error = new()
                {
                    Code = "CE01",
                    Description = "User not found."
                };
                return ConfirmEmailResult.Failed([error]);
            }

            IdentityResult result = await userManager.ConfirmEmailAsync(user, request.Code);

            if (!result.Succeeded)
            {
                var errors = result.Errors.ToArray();
                return ConfirmEmailResult.Failed(errors);
            }

            return ConfirmEmailResult.Success;
        }
        catch (Exception ex)
        {
            IdentityError error = new()
            {
                Code = "CE01",
                Description = $"An error occurred in {nameof(ConfirmEmailCommandHandler)}. Error details: {ex.Message}"
            };
            return ConfirmEmailResult.Failed([error]);
        }
    }
}
