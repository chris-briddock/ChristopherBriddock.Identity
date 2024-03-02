# ChristopherBriddock.Service.Identity

The Identity Service is a robust authentication and authorization component for your application, developed using ASP.NET 8 and integrating various libraries to ensure secure identity management.

![Azure DevOps builds](https://img.shields.io/azure-devops/build/chris1997/91f2d938-549b-497e-980d-188da969448a/7)
![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/chris1997/91f2d938-549b-497e-980d-188da969448a/7)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=chris-briddock_ChristopherBriddock.Identity&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=chris-briddock_ChristopherBriddock.Identity)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=chris-briddock_ChristopherBriddock.Identity&metric=bugs)](https://sonarcloud.io/summary/new_code?id=chris-briddock_ChristopherBriddock.Identity)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=chris-briddock_ChristopherBriddock.Identity&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=chris-briddock_ChristopherBriddock.Identity)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=chris-briddock_ChristopherBriddock.Identity&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=chris-briddock_ChristopherBriddock.Identity)

If you have used this project and want to support my open source projects consider [buying me a coffee](https://www.buymeacoffee.com/chrisbriddock)

Technical documentation generated with DocFX is [here](https://docs.cdjb.uk/api/ChristopherBriddock.Service.Common.Constants.html)

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
|  Authorize   |
|  Confirm Email |
| Delete Account |
|  Logout |
| Refresh Bearer Token |
| Register |
|  Update Password |
| Reset Password |
| Update Email Address |
| Two Step Verification |
| Two Factor Recovery Codes |
|  Account Lockout on 3 retries, for 10 minutes. |

### Technical Functionality

|   Features   |
| ----------- |
| RabbitMQ and Azure Service Bus support |
| API Health Checks |
| Secure JWT Bearer Authentication |
| Azure Application Insights Monitoring |
| Logging using Serilog (To Console Or To External Logging Server) e.g Seq or ElasticSearch |
| Resilliant Postgres connections using Entity Framework Core |
| Encrypted User Passwords |
| Uses HTTP/3 and fallsback to HTTP/2 or HTTP/1.1 |

### Endpoints

* **/authorize**: Endpoint for user authorization.
* **/sendemail**: Endpoint for sending token emails.
* **/confirmemail**: Endpoint for confirming user email.
* **/forgotpassword**: Endpoint for handling forgotten passwords.
* **/logout**: Endpoint for user logout.
* **/refresh**: Endpoint for refreshing authentication tokens.
* **/register**: Endpoint for user registration.
* **/resetpassword**: Endpoint for resetting user passwords.
* **/token**: Endpoint for token generation.
* **/2fa/authorize**: Endpoint for two-factor authentication authorization.
* **/2fa/manage**: Endpoint for managing two-factor authentication settings.
* **/2fa/codes**: Endpoint for generating and managing two-factor authentication codes.
* **/2fa/redeem**: Endpoint for reedming two-factor recovery codes.
* **/account/email**: Endpoint for managing user account email.
* **/account/phonenumber**: Endpoint for managing user account phone number.
* **/account/password**: Endpoint for managing user account password.
* **/account/delete**: Endpoint for deleting their user account.

### Getting Started

To get started with the Identity Service, follow these steps:

1. Clone the repository: `git clone https://github.com/chris-briddock/ChristopherBriddock.Identity.git`
2. Open the solution.
3. Ensure you have added migrations for the Service.Identity project with Entity Framework Core.
4. Ensure you have an PostgreSQL database and RabbitMQ or Azure Service Bus available at minimum
5. Optionally you can also use Seq, Azure Application Insights and Redis.
6. Please configure all placeholder values in appsettings.json in both services.
7. Build and run the Web API.

### Infrastructure as Code

The project includes infrastructure as code (IaC) configurations for provisioning a Kubernetes cluster in three major cloud platforms - Azure, AWS, and Google Cloud.

Below are the detailed instructions on how to provision infrastructure on each cloud provider.

#### Azure

The Azure cloud configuration can be found in the 'azure' directory. This uses Pulumi to define and provision Azure resources. To deploy a Kubernetes cluster in Azure:
  
1. Navigate to the 'azure' directory.
2. Install the necessary dependencies, you need to have Pulumi CLI and Node.js installed.
3. Run `pulumi stack init` to create a new Pulumi stacking environment.
4. Run `pulumi config set azure:location <AzureRegion>` to set your desired Azure region.
5. Run `pulumi up` to provision the resources.

#### AWS

Amazon Web Services (AWS) configuration is located in the 'aws' directory. Follow the steps below:

1. Navigate to the 'aws' directory.
2. Make sure all the dependencies are installed and AWS credentials are properly set in your environment.
3. Run `pulumi stack init` to create a new Pulumi stacking environment.
4. Run `pulumi up` to start the provisioning of resources.

#### Google Cloud

The Google Cloud configuration can be found in the 'googlecloud' directory. Follow the steps below:

1. Navigate to the 'googlecloud' directory.
2. Run `npm i` to install dependencies.
3. Run `pulumi stack init` to create a new Pulumi stacking environment.
4. Run `pulumi config set gcp:project <GoogleCloudProjectId>` and `pulumi config set gcp:region <GoogleCloudRegion>` to set your Google Cloud project id and desired Google Cloud region.
5. Install the Google Cloud CLI.
6. Log in to Google Cloud.
7. Run `pulumi up` to start the provisioning of resources.

After executing these steps on your desired cloud platform, a Kubernetes cluster will be provisioned and ready-to-use.

### License

This project is licensed under the ![GitHub License](https://img.shields.io/github/license/chris-briddock/ChristopherBriddock.Identity)
See the [LICENSE](LICENSE) file for details.
