global using Xunit;
global using Moq;
global using Shouldly;
global using MyResults;
global using ID.Tests.Data.Factories;
global using ID.Tests.Data.Factories.Dtos;
global using Google.Apis.Auth;
global using ID.Application.AppAbs.ApplicationServices.TwoFactor;
global using ID.Application.AppAbs.TokenVerificationServices;
global using ID.Application.JWT;
global using ID.Domain.Entities.AppUsers;
global using ID.Domain.Entities.Teams;
global using ID.Domain.Models;
global using ID.OAuth.Google.Data;
global using ID.OAuth.Google.Features.SignIn;
global using ID.OAuth.Google.Features.SignIn.GoogleSignIn;
global using ID.OAuth.Google.Services.Abs;
