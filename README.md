# ChristopherBriddock.Service.Identity

The Identity Service is a robust authentication and authorization component for your application, developed using ASP.NET 8 and integrating various libraries to ensure secure identity management.

![Azure DevOps builds](https://img.shields.io/azure-devops/build/chris1997/91f2d938-549b-497e-980d-188da969448a/7)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/chris1997/91f2d938-549b-497e-980d-188da969448a/7)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=chris-briddock_ChristopherBriddock.Identity&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=chris-briddock_ChristopherBriddock.Identity)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=chris-briddock_ChristopherBriddock.Identity&metric=bugs)](https://sonarcloud.io/summary/new_code?id=chris-briddock_ChristopherBriddock.Identity)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=chris-briddock_ChristopherBriddock.Identity&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=chris-briddock_ChristopherBriddock.Identity)

Technical documentation generated with DocFX is [here](https://chris-briddock.github.io/ChristopherBriddock.Identity/api/ChristopherBriddock.Service.Common.Constants.html)

## Table of Contents

* [Introduction](#introduction)
* [User Functionality](#user-functionality)
* [Technical Functionality](#technical-functionality)
* [Endpoints](#endpoints)
* [Getting Started](#getting-started)
* [License](#license)

### Introduction

The Identity Service plays a crucial role in your application's security infrastructure, providing essential features for user authentication, authorization, and identity management. This README provides an overview of the service, libraries used, available endpoints, and instructions on getting started.

### User Functionality

|  Features   |
| ----------- |
|  Register |
|  Authorise   |
| Delete Current User |
|  Update Current User Password |
| Reset Current User Password |
| Update Current User Email Address |
|  Account Lockout on 3 retries, for 10 minutes. |
| Two Step Verification |
| Refresh Bearer Token |
| Two Factor Recovery Codes |

### Technical Functionality

|   Features   |
| ----------- |
| RabbitMQ and Azure Service Bus support |
| API Health Checks |
| Secure JWT Bearer Authentication |
| Azure Application Insights Monitoring |
| Logging using Serilog (To Console Or To External Logging Server) e.g Seq or ElasticSearch |
| Resilliant SQL Server connections using Entity Framework Core |
| Encrypted User Passwords |
| Uses HTTP/3 and fallsback to HTTP/2 or HTTP/1.1 |

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
* **/account/delete**: Endpoint for deleting their user account.

### Getting Started

To get started with the Identity Service, follow these steps:

1. Clone the repository: `git clone https://github.com/chris-briddock/ChristopherBriddock.Identity.git`
2. Open the solution.
3. Ensure you have added migrations for the Service.Identity project with Entity Framework Core.
4. Ensure you have an PostgreSQL database and RabbitMQ or Azure Service Bus available at minimum. 
5. Optionally you can also use Seq, Azure Application Insights and Redis.
6. Please configure all placeholder values in appsettings.json in both services.
7. Build and run the Web API.

### License

This project is licensed under the ![GitHub License](https://img.shields.io/github/license/chris-briddock/ChristopherBriddock.Identity) 
See the [LICENSE](LICENSE) file for details.
