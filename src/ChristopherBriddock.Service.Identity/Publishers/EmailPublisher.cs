using MassTransit;

namespace ChristopherBriddock.Service.Identity.Publishers;

/// <summary>
/// Publishes a message to the message queue, for confirmation emails, 
/// password reset codes, password reset links and two factor codes.
/// </summary>
public sealed class EmailPublisher(IBus bus) : IEmailPublisher
{
    /// <summary>
    /// A bus is a logical element that includes a local endpoint and zero or more receive endpoints
    /// </summary>
    public IBus Bus { get; } = bus;

    /// <summary>
    /// Publishes a message to the message queue.
    /// </summary>
    /// <param name="emailMessage">The object which encapsulates the email message.</param>
    /// <param name="cancellationToken">The cancellation token which propigates notification that the operation will be cancelled.</param>
    /// <returns>An asyncronous operation of type <see cref="Task"/></returns>
    public async Task Publish(EmailMessagePublisher emailMessage, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Bus.Publish(emailMessage, cancellationToken);
        }
    }

}

/// <summary>
/// Represent an message to be published to the message queue for emails.
/// </summary>
public sealed class EmailMessagePublisher
{
    /// <summary>
    /// The email address to which the email should be sent to.
    /// </summary>
    public string EmailAddress { get; set; } = default!;
    /// <summary>
    /// The type of email to be sent to the message queue. 
    /// </summary>
    public string Type { get; set; } = default!;
    /// <summary>
    /// The code for two factor, forgotpassword, and registration.
    /// </summary>
    public string Code { get; set; } = default!;
}
