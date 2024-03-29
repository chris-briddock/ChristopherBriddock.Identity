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
                           IConfiguration configuration) : IConsumer<EmailMessage>
{
    /// <summary>
    /// The application logger.
    /// </summary>
    public ILogger<Worker> Logger { get; } = logger;
    /// <summary>
    /// The application configuration.
    /// </summary>
    public IConfiguration Configuration { get; } = configuration;

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
        var smtpServer = Configuration["Email:Server"]!;
        int smtpPort = Convert.ToInt16(Configuration["Email:Port"]!);
        var smtpUsername = Configuration["Email:Credentials:EmailAddress"]!;
        var smtpPassword = Configuration["Email:Credentials:Password"]!;

        using var client = new SmtpClient(smtpServer, smtpPort);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpUsername),
            IsBodyHtml = true
        };

        switch (context.Message.Type)
        {
            case EmailPublisherConstants.ConfirmEmail:
                mailMessage.Subject = $"Please confirm your email address.";
                mailMessage.Body = $@"<!DOCTYPE html>
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
                mailMessage.Subject = $"You requested a two factor code";
                mailMessage.Body = $@"<!DOCTYPE html>
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
                mailMessage.Subject = $"Oh no! You silly goose, you forgot your password. You can reset it here.";
                mailMessage.Body = $@"<!DOCTYPE html>
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
        mailMessage.To.Add(context.Message.EmailAddress);

        await client.SendMailAsync(mailMessage);
    }
}