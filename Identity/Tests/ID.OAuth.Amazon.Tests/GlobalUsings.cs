global using Xunit;
global using Moq;
global using Shouldly;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using MyResults;
global using System.Net;
global using System.Text.Json;

global using ID.OAuth.Amazon.Data;
global using ID.OAuth.Amazon.HttpService.Abs;
global using ID.OAuth.Amazon.HttpService.Imps;
global using ID.OAuth.Amazon.Setup;
global using ID.OAuth.Amazon.Services.Abs;
global using ID.OAuth.Amazon.Services.Imps;

global using ID.Domain.Entities.AppUsers;
global using ID.Domain.Entities.Teams;
global using ID.Domain.Models;
global using ID.Application.AppAbs.ApplicationServices.TwoFactor;
global using ID.Application.AppAbs.SignIn;
global using ID.Tests.Data.Factories;
global using ID.Tests.Utility.Logging;

global using ID.OAuth.Utils.Serialization;
global using FluentValidation.TestHelper;
