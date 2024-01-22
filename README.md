# ChristopherBriddock.Service.Identity

The Identity Service is a robust authentication and authorization component for your application, developed using ASP.NET 8 and integrating various libraries to ensure secure identity management.

## Table of Contents

* [Introduction](#introduction)
* [Endpoints](#endpoints)
* [Getting Started](#getting-started)
* [License](#license)

### Introduction

The Identity Service plays a crucial role in your application's security infrastructure, providing essential features for user authentication, authorization, and identity management. This README provides an overview of the service, libraries used, available endpoints, and instructions on getting started.

### Endpoints

* **/authorise**: Endpoint for user authorization.
* **/confirmemail**: Endpoint for confirming user email.
* **/forgotpassword**: Endpoint for handling forgotten passwords.
* **/logout**: Endpoint for user logout.
* **/refresh**: Endpoint for refreshing authentication tokens.
* **/register**: Endpoint for user registration.
* **/resetpassword**: Endpoint for resetting user passwords.
* **/token**: Endpoint for token management.
* **/2fa/authorise**: Endpoint for two-factor authentication authorization.
* **/2fa/manage**: Endpoint for managing two-factor authentication settings.
* **/2fa/codes**: Endpoint for generating and managing two-factor authentication codes.
* **/2fa/redeem**: Endpoint for reedming two-factor recovery codes.
* **/2fa/email**: Endpoint for sending two factor code emails.
* **/account/email**: Endpoint for managing user account email.
* **/account/phonenumber**: Endpoint for managing user account phone number.
* **/account/password**: Endpoint for managing user account password.

### Getting Started

To get started with the Identity Service, follow these steps:

1. Clone the repository: `git clone https://github.com/chris-briddock/ChristopherBriddock.Identity.git`
2. Open the solution.
3. Please configure all placeholder values in appsettings.json in both services.
4. Build and run the Web API.

### License

This project is licensed under the [MIT License](LICENSE). See the LICENSE file for details.
