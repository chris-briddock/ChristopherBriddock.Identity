using ChristopherBriddock.Service.Common.Constants;
using ChristopherBriddock.Service.Common.Messaging;
using MassTransit;
using System.Net;
using System.Net.Mail;

namespace ChristopherBriddock.WorkerService.Email;

/// <summary>
/// Consume the message from the message queue, and sends an email.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="Worker"/> 
/// </remarks>
/// <param name="logger">The application logger.</param>
/// <param name="configuration">The configuration of the application. </param> 
public class Worker(ILogger<Worker> logger,
                    IServiceProvider serviceProvider) : IConsumer<EmailMessage>
{
    /// <summary>
    /// The application logger.
    /// </summary>
    public ILogger<Worker> Logger { get; } = logger;

    /// <summary>
    /// The service provider.
    /// </summary>
    /// <value></value>
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    /// <summary>
    /// Consumes a message from the message queue.
    /// </summary>
    /// <param name="context">The <see cref="ConsumeContext{T}"/> that allows for message consumption.</param>
    /// <remarks>
    /// This method is automatically executed, as MassTransit registers consumers and publishers (producers) 
    /// as a <see cref="BackgroundService"/> which implements <see cref="IHostedService"/> 
    /// </remarks>
    /// <returns>An asyncronous <see cref="Task"/></returns>
    public async Task Consume(ConsumeContext<EmailMessage> context)
    {
        var configuration = ServiceProvider.GetService<IConfiguration>()!;
        var from = configuration["Email:Credentials:EmailAddress"]!;
        var to = context.Message.EmailAddress;
        var message = new MailMessage(from, to) 
        {
            IsBodyHtml = true 
        };
        var client = ServiceProvider.GetService<ISmtpClient>()!;

        switch (context.Message.Type)
        {
            case EmailPublisherConstants.ConfirmEmail:
                message.Subject = $"Please confirm your email address.";
                message.Body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Two-Factor Verification Code</title>
    <!-- Include Tailwind CSS styles -->
    <link href=""https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css"" rel=""stylesheet"">
</head>
<body class=""font-sans bg-gray-100"">
    <div class=""max-w-screen-md mx-auto p-8 bg-white shadow-md rounded-md"">
        <h2 class=""text-2xl font-semibold mb-4 text-gray-800"">Confirm your email</h2>
        <p class=""text-gray-700"">Dear <span class=""font-bold text-indigo-800"">{context.Message.EmailAddress}</span>,</p>
        <p class=""text-gray-700"">Your confirmation email link is <a href=""{context.Message.Link}"" class=""text-indigo-600"">here</a></p>
        <p class=""text-gray-700"">If you did not request this or have any concerns, please contact our support team.</p>
        <p class=""mt-4 text-gray-700"">Thank you</p>
        <p class=""mt-2 text-gray-600"">� 2024 All rights reserved.</p>
    </div>
</body>
</html>";
                break;
            case EmailPublisherConstants.TwoFactorToken:
                message.Subject = $"You requested a two factor code";
                message.Body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Two-Factor Verification Code</title>
    <!-- Include Tailwind CSS styles -->
    <link href=""https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css"" rel=""stylesheet"">
</head>
<body class=""font-sans bg-gray-100"">
    <div class=""max-w-screen-md mx-auto p-8 bg-white shadow-md rounded-md"">
        <h2 class=""text-2xl font-semibold mb-4 text-gray-800"">Two-Factor Verification Code</h2>
        <p class=""text-gray-700"">Dear <span class=""font-bold text-indigo-800"">{context.Message.EmailAddress}</span>,</p>
        <p class=""text-gray-700"">Your two factor code is {context.Message.Code}</p>
        <p class=""text-gray-700"">If you did not request this or have any concerns, please contact our support team.</p>
        <p class=""mt-4 text-gray-700"">Thank you,<br>Your Company Name</p>
        <p class=""mt-2 text-gray-600"">� 2024 All rights reserved.</p>
    </div>
</body>
</html>";
                break;
            case EmailPublisherConstants.ForgotPassword:
                message.Subject = $"Oh no! You silly goose, you forgot your password. You can reset it here.";
                message.Body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Two-Factor Verification Code</title>
    <!-- Include Tailwind CSS styles -->
    <link href=""https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css"" rel=""stylesheet"">
</head>
<body class=""font-sans bg-gray-100"">
    <div class=""max-w-screen-md mx-auto p-8 bg-white shadow-md rounded-md"">
        <h2 class=""text-2xl font-semibold mb-4 text-gray-800"">Forgotten your password.</h2>
        <p class=""text-gray-700"">Dear <span class=""font-bold text-indigo-800"">{context.Message.EmailAddress}</span>,</p>
        <p class=""text-gray-700"">Your password reset code is <span class=""font-bold text-indigo-800"">{context.Message.Code}</span> </p>
        <p class=""text-gray-700"">If you did not request this or have any concerns, please contact our support team.</p>
        <p class=""mt-4 text-gray-700"">Thank you,<br>Your Company Name</p>
        <p class=""mt-2 text-gray-600"">� 2024 All rights reserved.</p>
    </div>
</body>
</html>";
                break;
            default:
                break;
        }

        await client.SendMailAsync(message);
    }
}