global using ChristopherBriddock.Service.Identity.Models;
global using ChristopherBriddock.Service.Identity.Models.Requests;
global using ChristopherBriddock.Service.Identity.Tests.Mocks;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Moq;
global using System.Net;
global using System.Net.Http.Json;
global using NUnit.Framework;
global using ChristopherBriddock.Service.Identity.Data;
global using ChristopherBriddock.Service.Identity.Services;
global using Microsoft.Extensions.Logging;
global using Testcontainers.MsSql;
global using DotNet.Testcontainers.Builders;
global using Microsoft.Extensions.Configuration;
global using DotNet.Testcontainers.Containers;
global using Microsoft.AspNetCore.Http;
global using System.Net.Http.Headers;
global using System.Security.Claims;
global using ChristopherBriddock.Service.Identity.Models.Results;
global using ChristopherBriddock.Service.Identity.Providers;
global using Microsoft.AspNetCore.Identity.Data;
global using ChristopherBriddock.Service.Common.Constants;
global using ChristopherBriddock.Service.Common.Messaging;
global using ChristopherBriddock.Service.Identity.Publishers;
global using System.Text.Encodings.Web;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.Extensions.Options;
global using System.Text.Json;
global using ChristopherBriddock.Service.Identity.Models.Responses;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
