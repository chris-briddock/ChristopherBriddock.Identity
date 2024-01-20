# ChristopherBriddock.Service.Identity

The Identity Service is a robust authentication and authorization component for your application, developed using ASP.NET 8 and integrating various libraries to ensure secure identity management.

## Table of Contents

* [Introduction](#introduction)
* [Endpoints](#endpoints)
* [Getting Started](#getting-started)
* [Configuration](#configuration)
* [Libraries Used](#libraries-used)
* [License](#license)

### Introduction

The Identity Service plays a crucial role in your application's security infrastructure, providing essential features for user authentication, authorization, and identity management. This README provides an overview of the service, libraries used, available endpoints, and instructions on getting started.

### Endpoints

* **/authorize**: Endpoint for user authorization.
* **/confirmemail**: Endpoint for confirming user email.
* **/forgotpassword**: Endpoint for handling forgotten passwords.
* **/logout**: Endpoint for user logout.
* **/refresh**: Endpoint for refreshing authentication tokens.
* **/register**: Endpoint for user registration.
* **/resetpassword**: Endpoint for resetting user passwords.
* **/token**: Endpoint for token management.
* **/2fa/authorize**: Endpoint for two-factor authentication authorization.
* **/2fa/manage**: Endpoint for managing two-factor authentication settings.
* **/2fa/codes**: Endpoint for generating and managing two-factor authentication codes.
* **/2fa/redeem**: Endpoint for redeethenticaming two-factor aution codes.
* **/2fa/email**: Endpoint for sending two factor code emails.
* **/account/email**: Endpoint for managing user account email.
* **/account/phonenumber**: Endpoint for managing user account phone number.
* **/account/password**: Endpoint for managing user account password.

### Getting Started

To get started with the Identity Service, follow these steps:

1. Clone the repository: `git clone https://github.com/chris-briddock/ChristopherBriddock.Identity`
2. Open the solution.
3. Please refer to the [Configuration](#configuration) section.
4. Build and run the Web API.

### Configuration

    1. Configure PostgreSQL in appsettings.json, under ConnectionStrings > Default.
    2. Optionally configure Redis with a connection string and instance name.
    3. Update the JWT section, with audience, secret and expires values.
    4. Update FeatureManagement section, if you would like to use Redis or Application Insights.
    5. Update ApplicationInsights section with your instrumentation key.
    6. If you have a Seq server available update the "WriteTo" section of Serilog.

### Libraries Used

1. **ASP.NET 8**: Primary web framework for building APIs in C#.
2. **Ardallis.Endpoints**: Provides a clean and testable way to define API endpoints.
3. **AspNetCore.HealthChecks.NpgSql**: Enables health checks for PostgreSQL databases. Usage is specific to C# applications.
4. **AspNetCore.HealthChecks.Publisher.Seq**: Extends health checks by providing a publisher for Seq logging. Seq is a centralized log server.
5. **AspNetCore.Redis**: Library for integrating Redis caching in ASP.NET Core applications. Usage is specific to C# applications.
6. **MassTransit**: A distributed application framework for .NET. Provides abstractions for messaging patterns.
7. **MassTransit.RabbitMQ**: Transport extension for MassTransit, specifically for RabbitMQ. Enables message-based communication.
8. **Microsoft.ApplicationInsights.AspNetCore**: Integrates Azure Application Insights into ASP.NET Core applications for monitoring and diagnostics.
9. **Microsoft.ApplicationInsights.Kubernetes**: Extends Application Insights for monitoring applications deployed on Kubernetes.
10. **Microsoft.AspNetCore.Authentication.JwtBearer**: Middleware for validating JSON Web Tokens (JWTs) in ASP.NET Core.
11. **Microsoft.AspNetCore.Identity.EntityFrameworkCore**: ASP.NET Core Identity provider using Entity Framework Core. Manages user identity and authorization.
12. **Microsoft.EntityFrameworkCore**: Object-relational mapper (ORM) for simplifying database interactions in .NET applications.
13. **Microsoft.FeatureManagement**: Library for feature management, controlling feature availability through configuration.
14. **Microsoft.IdentityModel.JsonWebTokens**: Library for working with JSON Web Tokens (JWTs). Essential for authentication and authorization.
15. **NpgSql.EntityFrameworkCore.PostgreSQL**: PostgreSQL database provider for Entity Framework Core.
16. **Serilog**: Logging library for .NET applications. Provides structured logging.
17. **Serilog.AspNetCore**: Integration library for using Serilog with ASP.NET Core applications.
18. **Serilog.Sink.Console**: Serilog sink for writing log events to the console. Useful for development and debugging.
19. **Serilog.Sinks.Seq**: Serilog sink for Seq logging. Seq is a centralized logging server.
20. **NSubstitute**: Mocking library for .NET. Used in unit tests to create substitute objects.
21. **Xunit**: Testing framework for .NET applications. Provides a simple and extensible approach to writing unit tests in C#.

### License

This project is licensed under the [MIT License](LICENSE). See the LICENSE file for details.
