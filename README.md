# MyId Identity System

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0-blue.svg)](https://docs.microsoft.com/en-us/ef/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

MyId is a comprehensive, enterprise-grade identity management system built with Clean Architecture principles, Domain-Driven Design (DDD), and modern .NET 8 technologies. It provides secure authentication, authorization, team management, and extensible identity services for modern applications.

## 🏗️ Architecture Overview

The system follows Clean Architecture with clear separation of concerns:

```
Identity/
├── API/                    # Web API layer (Controllers, Endpoints)
├── Core/                   # Core business logic
│   ├── ID.Application/     # Application services, handlers, features
│   ├── ID.Domain/          # Domain entities, value objects, abstractions
│   ├── ID.GlobalSettings/  # Configuration and settings
│   └── ID.IntegrationEvents/ # Event-driven communication
├── Infrastructure/         # External concerns
│   ├── ID.Infrastructure/  # Data access, services, integrations
│   └── ID.Presentation/    # Controllers and presentation logic
├── AddOns/                 # Optional extensions
│   ├── Emails/            # Email providers (SendGrid, SMTP)
│   ├── ID.PhoneConfirmation/ # Phone verification
│   ├── Msgs/              # Messaging (Twilio)
│   └── OAuth/             # OAuth providers (Google)
├── Extras/                 # Additional utilities & extensions
│   ├── ID.TeamRoles.UserToAdmin/ # Custom claims and role elevation policies
│   └── JwtKeyBuilder/     # JWT key generation utilities
│       ├── ID.Jwt.Builder.KeyBuilder/   # Core key generation library
│       ├── ID.Jwt.Builder.Console/      # Console app for key generation
│       └── ID.Jwt.KeyValidator.Console/ # Key validation utility
└── Tests/                  # Comprehensive test suite
```

## 🚀 Key Features

### Core Identity Services
- **Authentication & Authorization**: JWT-based with refresh tokens
- **User Management**: Registration, login, profile management
- **Team Management**: Hierarchical team structures with subscriptions
- **Device Tracking**: Multi-device session management
- **Password Policies**: Configurable security requirements

### Enterprise Features
- **Multi-Tenant Support**: Team-based isolation
- **Role-Based Access Control**: Flexible permission system
- **Audit Trail**: Complete activity logging
- **Background Jobs**: Hangfire integration for async processing
- **Event-Driven Architecture**: Domain events and integration events

### Optional Extensions
- **Email Services**: SendGrid and SMTP providers
- **SMS/Phone Confirmation**: Twilio integration
- **OAuth Providers**: Google authentication
- **Customer Management**: Specialized customer workflows

## 🛠️ Technology Stack

### Core Technologies
- **.NET 8**: Latest framework features and performance
- **Entity Framework Core 8**: Advanced ORM with migrations
- **ASP.NET Core Identity**: Built-in authentication framework
- **MediatR**: CQRS and mediator pattern implementation

### Data & Persistence
- **SQL Server**: Primary database (Azure SQL supported)
- **Entity Framework**: Code-first migrations
- **Repository Pattern**: Clean data access abstraction
- **Specifications Pattern**: Composable query logic

### Security & Authentication
- **JWT Tokens**: Asymmetric key signing (RSA)
- **Refresh Tokens**: Secure session management
- **Claims-Based Security**: Fine-grained permissions
<!-- - **HTTPS Enforcement**: Secure communication -->

### Background Processing
- **Hangfire**: Reliable background job processing
- **Dashboard Integration**: Job monitoring and management
- **Recurring Jobs**: Scheduled maintenance tasks

### Testing & Quality
- **xUnit**: Unit and integration testing
- **Shouldly**: Fluent assertions
- **In-Memory Provider**: Fast test execution
- **Test Data Factories**: Consistent test data generation

## 📋 Prerequisites

### Development Environment
- **.NET 8 SDK** or later
- **Visual Studio 2022** (17.8+) or **VS Code** with C# extension
- **SQL Server** (LocalDB, Express, or full version)
- **Git** for version control

### NuGet Package Requirements
All package versions are centrally managed in `Directory.Packages.props`:
- Entity Framework Core 8.0.16
- ASP.NET Core Identity 8.0.16
- MediatR 12.5.0
- Hangfire 1.8.20
- JWT Bearer 8.0.16

## 🚀 Quick Start

### 1. Clone and Setup

```bash
git clone <repository-url>
cd MyId
```

### 2. Database Configuration

Currently set up to use MSSQL, other DBs to follow
Update your connection string in `appsettings.json`: or wherever you keep it. 
Can be the same or separate DB from the main app DB. Scheam is MyId
```json
{
  "ConnectionStrings": {
    "SqlDb": "Server=(localdb)\\mssqllocaldb;Database=MyIdDemo;Trusted_Connection=true;MultipleActiveResultSets=true;"
  }
}
```

### 3. Install the Identity System

In your ASP.NET Core application, use the `MyIdInstaller`:

```csharp
// Program.cs

var builder = WebApplication.CreateBuilder(args);

//Or however you addess your setup data (appsettings, Environvent variables, etc)
//See MyIdDemo for StartupData class
var startupData = new StartupData(builder.Configuration, builder.Environment); 

// Install MyId services and Configure Identity Options
   //builder.Services.AddMyId<TeamRole_User_to_Mgr_ClaimsGenerator>( // Uncomment this to use the TeamRoles User to Admin feature
  builder.Services.AddMyId(
     config =>
     {
         config.ApplicationName = startupData.APP_NAME;
         config.ConnectionString = startupData.ConnectionStringsSection.GetSqlDb();
         //config.TokenSigningKey = startupData.IdentitySection.GetSymetricKey();
         config.AsymmetricPrivateKey_Xml = startupData.GetAsymmetricPrivateKeyXmlString();
         config.AsymmetricPublicKey_Xml = startupData.GetAsymmetricPublicKeyXmlString();
         config.TokenExpirationMinutes = startupData.IdentitySection.GetJwtExpirationMinutes();
         config.RefreshTokensEnabled = true;
         config.MntcAccountsUrl = startupData.IdentitySection.GetMntcRoute();
         config.AllowExternalPagesDevModeAccess = false;
         config.FromMoblieAppHeaderValue = "MobileApp-TestValue";
         config.UseSeperateEventBus = true;
         config.PasswordOptions = new ID.Infrastructure.Setup.Passwords.IdPasswordOptions()
         {
             RequireDigit = true,
             RequireLowercase = true,
             RequireNonAlphanumeric = true,
             RequireUppercase = true,
             RequiredLength = 6,
             RequiredUniqueChars = 1
         };
     })
     .Services
     .AddMyIdMessagingTwilio(config =>
     {
         config.TwilioFromNumber = startupData.TwilioOptionsSection.GetFrom();
         config.TwilioPassword = startupData.TwilioOptionsSection.GetPassword();
         config.TwilioId = startupData.TwilioOptionsSection.GetId();
     })
     .AddMyIdCustomers(config =>
     {
         config.CustomerAccountsUrl = startupData.IdentitySection.GetCustomerRoute();
     })
     .AddMyIdPhoneConfirmation()
     //.AddTeamRolesUserToAdmin()// Uncomment this to add the TeamRoles User to Admin Policies
     ;




  if (builder.Environment.IsDevelopment())
  {
      builder.Services.AddMyIdEmailSmtp(config =>
      {
          config.SmtpServerAddress = startupData.SmtpSettingsSection.GetServerAddress();
          config.SmtpPortNumber = startupData.SmtpSettingsSection.GetPortNumber();
          config.SmtpUsernameOrEmail = startupData.SmtpSettingsSection.GetUsername();
          config.SmtpPassword = startupData.SmtpSettingsSection.GetPassword();

          config.BccAddresses = startupData.IdentitySection.GetBccAddresses()!;
          config.LogoUrl = startupData.GetLogoUrl();
          config.FromAddress = startupData.IdentitySection.GetFromAddress()!;
          config.ColorHexBrand = startupData.COLOR_HEX_BRAND;
          config.FromName = startupData.IdentitySection.GetFromName();
      });
  }
  else
  {

      builder.Services.AddMyIdEmailSG(config =>
      {
          config.ApiKey = startupData.SendGridSettingsSection.GetApiKey()!;

          config.BccAddresses = startupData.IdentitySection.GetBccAddresses()!;
          config.FromAddress = startupData.IdentitySection.GetFromAddress()!;
          config.LogoUrl = startupData.GetLogoUrl();
          config.ColorHexBrand = startupData.COLOR_HEX_BRAND;
          config.FromName = startupData.COMPANY_NAME;
      });
  }

var app = builder.Build();

// Configure MyId middleware
app.UseMyId();

app.Run();
```

### 4. Initialize the System

Start the application and call the initialization endpoint:

```bash
# Start the application
dotnet run
```

```http
POST /identity/IdMaintenance/Initialize
Content-Type: application/json

{
  "password": "YourSuperLeaderPassword123!",
  "email": "admin@yourcompany.com"  // Optional - will use default if not provided
}
```

The initialization process:
- **Automatically migrates** the database (no manual migration needed)
- **Creates the Super Team** with system-level permissions
- **Creates the Super Leader** user with the provided credentials
- **Creates the Maintenance Team** for administrative operations
- **Confirms the Super Leader's email** automatically
- **Returns the Super Leader email** for confirmation

The Super Leader can then:
- Add other Super users to the Super Team
- Add Maintenance users to the Maintenance Team
- Configure additional system settings

The endpoint /identity/IdMaintenance/Initialize 'self-destructs' after initialization is complete

### 5. Access the System

Navigate to `https://localhost:[devport]/swagger` to explore the API, or use the Super Leader credentials to start managing your identity system.

## 🔧 Configuration Guide

### JWT Token Configuration

Generate RSA key pairs for token signing:

```csharp
// Use the included JWT Key Builder
// Located in: Identity/Extras/JwtKeyBuilder/
```

### Database Configuration

The system supports MSQQL more providers to follow:

### Email Configuration
The System has some pre-build email providers with event listeners to react to forgot-password-email, new-user etc events.


#### SMTP (Development)
```csharp
      builder.Services.AddMyIdEmailSmtp(config =>
      {
          config.SmtpServerAddress = startupData.SmtpSettingsSection.GetServerAddress();
          config.SmtpPortNumber = startupData.SmtpSettingsSection.GetPortNumber();
          config.SmtpUsernameOrEmail = startupData.SmtpSettingsSection.GetUsername();
          config.SmtpPassword = startupData.SmtpSettingsSection.GetPassword();

          config.BccAddresses = startupData.IdentitySection.GetBccAddresses()!;
          config.LogoUrl = startupData.GetLogoUrl();
          config.FromAddress = startupData.IdentitySection.GetFromAddress()!;
          config.ColorHexBrand = startupData.COLOR_HEX_BRAND;
          config.FromName = startupData.IdentitySection.GetFromName();
      });
```

#### SendGrid (Production)
```csharp
  builder.Services.AddMyIdEmailSG(config =>
  {
      config.ApiKey = startupData.SendGridSettingsSection.GetApiKey()!;

      config.BccAddresses = startupData.IdentitySection.GetBccAddresses()!;
      config.FromAddress = startupData.IdentitySection.GetFromAddress()!;
      config.LogoUrl = startupData.GetLogoUrl();
      config.ColorHexBrand = startupData.COLOR_HEX_BRAND;
      config.FromName = startupData.COMPANY_NAME;
  });
```

### Hangfire Configuration

To authenticate your docker endpoint add UseHangfireDashboard() AFTER UseMyId().
If using Jwt token append your token as a paramater to to the docker endpoint
    - <hangfire-dashboard-url>?tkn=<jwt-goes-here>

```json
{
  "HangFire": {
    "DashboardPath": "/myid-hangfire",
    "DashboardTitle": "MyId Jobs Dashboard",
    "Schema": "MyIdJobs",
    "Server": "MyIdServer"
  }
}
```

## 📚 Domain Model

### Core Entities

#### Team Management
- **Team**: Organizational units with hierarchical support
    - 3 Team Types
        - Super : Single Team of Devs. Highest access level. (Createed on initialization)
        - Maintenance : Single Team of Maintenance Users. Second highest access level. (Createed on initialization)
            This will be the owners of the app.
        - Customers: Options for applicaitons with customers. Default team size of 1.

- **TeamSubscription**: Team-based feature subscriptions
- **Device**: Multi-device session tracking
- **SubscriptionPlan**: Available feature sets and pricing

#### User Management
- **AppUser**: Core user entity with ASP.NET Core Identity integration. Always in a memmber of a team.
    Super, Maintenance or Customer 
- **Avatar**: User profile images and display customization
- **RefreshToken**: Secure session management


#### System
- **OutboxMessage**: Reliable event publishing
- **IIdAuditableDomainEntity**: Activity tracking and compliance (All domain entities inherit for tracking)

### Value Objects Pattern

The system uses a hybrid approach for domain modeling:

We use Value objects for creating and updating but the entity stores primitives.
The value objects payload used to set the Entity Property in the constructor.
    - Single source of failure.


```csharp
public class Team : AggregateRoot<TeamId>
{
    // Primitive properties for EF Core compatibility
    public string TeamName { get; private set; }
    
    // Value objects for input validation
    public void UpdateTeamName(Name teamName, IUpdateTeamNameValidator validator)
    {
        ArgumentNullException.ThrowIfNull(teamName);
        validator.ValidateCanUpdateTeamName(this);
        TeamName = teamName.Value; // Store primitive value
        AddDomainEvent(new TeamNameUpdatedEvent(Id, teamName));
    }
}
```

Benefits:
- **Domain clarity** through value objects
- **EF Core compatibility** with primitive storage
- **Query performance** without translation issues

## 🔒 Security Features

### Authentication Flow

1. **Registration**: User creates account with email verification
2. **Login**: Credentials validated, JWT token or Cookie issued
3. **Token Refresh**: Automatic token renewal with refresh tokens
4. **Multi-Device**: Support for multiple concurrent sessions

### Authorization System

The system includes a comprehensive set of pre-built authentication filters for different authorization levels:

#### **Team-Based Authorization Filters**
```csharp
// Super Team members only (highest level)
[SuperMinimumAuthenticator.ActionFilter]
public async Task<IActionResult> CriticalSystemOperation() { }

// Super Team OR Development environment
[SuperMinimumOrDevAuthenticator.ActionFilter] 
public async Task<IActionResult> AdminOperation() { }

// Maintenance Team or higher (Super Team)
[MntcMinimumAuthenticator.ActionFilter]
public async Task<IActionResult> MaintenanceOperation() { }

// Customer Team Leaders or higher
[CustomerLeaderMinimumAuthenticator.ActionFilter]
public async Task<IActionResult> CustomerManagement() { }
```

#### **Position-Based Authorization**
```csharp
// Require specific team position level
[MntcMinimumAuthenticator.ActionFilter(level: 5)]
public async Task<IActionResult> SeniorMaintenance() { }

// Super team with minimum position
[SuperMinimumAuthenticator.ActionFilter(level: 3)]
public async Task<IActionResult> LeadDeveloper() { }
```

#### **Specialized Authenticators**
```csharp
// Only during system initialization (self-destructs after setup)
[InitializedAuthenticator.ActionFilter]
public async Task<IActionResult> Initialize() { }

// Team-specific operations
[TeamOwnerAuthenticator.ActionFilter]
public async Task<IActionResult> ManageTeam() { }
```

#### **Policy-Based Authorization**
```csharp
...
```

### Security Best Practices

- **Password Hashing**: ASP.NET Core Identity with configurable options
- **Token Expiration**: Configurable JWT/Cookie lifetime
- **Audit Logging**: Complete activity tracking

## 🔄 Event-Driven Architecture

### Domain Events

```csharp
public class TeamCreatedEvent : IDomainEvent
{
    public TeamId TeamId { get; }
    public Name TeamName { get; }
    public DateTime OccurredOn { get; }
    
    public TeamCreatedEvent(TeamId teamId, Name teamName)
    {
        TeamId = teamId;
        TeamName = teamName;
        OccurredOn = DateTime.UtcNow;
    }
}
```

### Integration Events

```csharp
public class UserRegisteredIntegrationEvent : IIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime RegisteredAt { get; set; }
}
```

### Event Handlers

```csharp
public class SendWelcomeEmailHandler : INotificationHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        await _emailService.SendWelcomeEmailAsync(notification.Email);
    }
}
```

## 🧪 Testing Strategy

### Test Structure

```
Tests/
├── ID.API.Tests/                    # API integration tests
├── ID.Application.Tests/            # Application service tests
├── ID.Domain.Tests/                 # Domain logic tests
├── ID.Infrastructure.Tests/         # Repository and infrastructure tests
└── ID.Tests.Data/                   # Test data factories
```

### Repository Testing Pattern

```csharp
[Fact]
public async Task AddAsync_Should_Add_Team_To_Database()
{
    // Arrange
    var team = TeamDataFactory.CreateValidTeam();
    
    // Act
    await _teamRepo.AddAsync(team);
    await _unitOfWork.SaveChangesAsync();
    
    // Assert
    var savedTeam = await _teamRepo.FirstOrDefaultAsync(new TeamByIdSpec(team.Id));
    savedTeam.Should().NotBeNull();
    savedTeam!.TeamName.Should().Be(team.TeamName);
}
```

### Test Data Factories

```csharp
public static class TeamDataFactory
{
    public static Team CreateValidTeam(string? name = null)
    {
        return Team.Create(
            new Name(name ?? "Test Team"),
            new Description("Test Description"),
            mockValidator.Object
        );
    }
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Identity/Tests/ID.Infrastructure.Tests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Performance & Monitoring

### Caching Strategy

#### System Teams Caching

```csharp
public class CachedTeamService : ITeamService
{
    private readonly IMemoryCache _cache;
    private readonly ITeamService _teamService;
    
    public async Task<Team?> GetSuperTeamAsync()
    {
        return await _cache.GetOrCreateAsync("super-team", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return await _teamService.GetSuperTeamAsync();
        });
    }
}
```

#### Cache Invalidation

```csharp
public class TeamUpdatedEventHandler : INotificationHandler<TeamUpdatedEvent>
{
    public async Task Handle(TeamUpdatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.TeamId == SystemTeamIds.SuperTeam)
        {
            _cache.Remove("super-team");
        }
    }
}
```

### Background Jobs
 - View in Hangfire dashboard

```csharp
[AutomaticRetry(Attempts = 3)]
public class CleanupExpiredTokensJob
{
    public async Task ExecuteAsync()
    {
        await _tokenService.CleanupExpiredTokensAsync();
    }
}

// Schedule recurring job
RecurringJob.AddOrUpdate<CleanupExpiredTokensJob>(
    "cleanup-tokens",
    x => x.ExecuteAsync(),
    Cron.Daily);
```
<!-- 
### Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<IdDbContext>()
    .AddHangfire(options => options.MaximumJobsFailed = 5);

app.MapHealthChecks("/health");
``` -->

## 🌐 API Documentation

### Authentication & Account Management

```http
# Registration & Login
POST   /identity/Account/RegisterCustomer
POST   /identity/Account/CreateCustomer
POST   /identity/Account/Login
POST   /identity/Account/LoginRefresh
POST   /identity/Account/CookieSignIn
POST   /identity/Account/CookieSignOut

# Email & Phone Confirmation
POST   /identity/Account/ConfirmEmail
POST   /identity/Account/ConfirmEmailWithPassword
POST   /identity/Account/ResendEmailConfirmation
GET    /identity/Account/ResendEmailConfirmation/{email}
POST   /identity/Account/ConfirmPhone
POST   /identity/Account/ResendPhoneConfirmation
GET    /identity/Account/ResendPhoneConfirmation/{email}

# Password Management
POST   /identity/Account/ForgotPassword
POST   /identity/Account/ChangePassword
POST   /identity/Account/ResetPassword

# Two-Factor Authentication
GET    /identity/Account/TwoFactorSetupData
POST   /identity/Account/TwoFactorVerification
POST   /identity/Account/TwoFactorResend
GET    /identity/Account/TwoFactorAuthAppComplete
GET    /identity/Account/TwoFactorAuthAppEmailComplete/{code}
PATCH  /identity/Account/EnableTwoFactorAuthentication
PATCH  /identity/Account/DisableTwoFactorAuthentication

# Profile & Account Info
GET    /identity/Account/MyInfo
GET    /identity/Account/MyInfoCustomer
DELETE /identity/Account/CloseAccount
DELETE /identity/Account/RefreshTokenRevoke
```

### Team Management

```http
# Team Operations
GET    /identity/Teams/GetAll
POST   /identity/Teams/Add
PATCH  /identity/Teams/Edit
DELETE /identity/Teams/Delete/{id}
GET    /identity/Teams/Get/{id}
GET    /identity/Teams/GetSuper
GET    /identity/Teams/GetMntc
GET    /identity/Teams/GetAllFiltered
GET    /identity/Teams/GetAllFiltered/{name}
POST   /identity/Teams/Page
PATCH  /identity/Teams/UpdatePositionRange

# Team Member Management
POST   /identity/Teams/AddCustomerToTeam
POST   /identity/Account/AddCustomerTeamMember
POST   /identity/Account/AddCustomerTeamMemberMntc
POST   /identity/Account/AddMntcTeamMember
POST   /identity/Account/AddSuperTeamMember

# Team Subscriptions
POST   /identity/Teams/AddSubscription
GET    /identity/Teams/GetSubscription/{subId}
POST   /identity/Teams/RemoveSubscription
POST   /identity/Teams/RemcordSubscriptionPayment

# Team Devices
GET    /identity/Teams/GetDevice/{subId}/{dvcId}
POST   /identity/Teams/AddDevice
POST   /identity/Teams/RemoveDevice
PATCH  /identity/Teams/UpdateDevice
```

### User Management

```http
# User Profile Management
PATCH  /identity/UserManagement/UpdateMember
PATCH  /identity/UserManagement/UpdatePosition/{userId}/{newPosition}
PATCH  /identity/UserManagement/UpdateAddress
PATCH  /identity/UserManagement/UpdateTwoFactorProvider/{provider}

# Team Leadership
PATCH  /identity/UserManagement/UpdateMyTeamLeader/{newLeaderId}
PATCH  /identity/UserManagement/UpdateLeader

# User Retrieval
GET    /identity/UserManagement/GetTeamMembers
GET    /identity/UserManagement/GetMntcTeamMembers
GET    /identity/UserManagement/GetSuperTeamMembers
GET    /identity/UserManagement/GetMyTeamMember/{userId}
GET    /identity/UserManagement/GetTeamMember/{teamId}/{userId}
GET    /identity/UserManagement/GetSuperTeamMember/{userId}
GET    /identity/UserManagement/GetMntcTeamMember/{userId}
GET    /identity/UserManagement/GetCustomers
GET    /identity/UserManagement/GetCustomer/{teamId}/{userId}

# User Deletion
DELETE /identity/UserManagement/DeleteCustomer/{userId}
DELETE /identity/UserManagement/DeleteMntcMember/{userId}
DELETE /identity/UserManagement/DeleteSuperMember/{userId}
DELETE /identity/UserManagement/DeleteTeamMember/{userId}

# Paginated User Lists
POST   /identity/UserManagement/GetMntcTeamMembersPage
POST   /identity/UserManagement/GetSuperTeamMembersPage
POST   /identity/UserManagement/GetCustomersPage
```

### Swagger Integration

The API includes comprehensive Swagger documentation with JWT authentication support:

To authenticate your docker endpoint add UseSwagger() AFTER UseMyId().
If using Jwt token append your token as a paramater to to the docker endpoint
    - swagger?tkn=<jwt-goes-here>


```csharp
c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JWT Authorization header using the Bearer scheme."
});
```

Access Swagger UI at: `https://localhost:5001/swagger`

## 🚀 Deployment

### Development Deployment

```bash
# Build the application
dotnet build

# Run migrations
dotnet ef database update

# Start the application
dotnet run --project Apps/MyIdDemo
```

### Production Deployment

<!-- #### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Apps/MyIdDemo/MyIdDemo.csproj", "Apps/MyIdDemo/"]
RUN dotnet restore "Apps/MyIdDemo/MyIdDemo.csproj"
COPY . .
WORKDIR "/src/Apps/MyIdDemo"
RUN dotnet build "MyIdDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyIdDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyIdDemo.dll"] -->
```

<!-- #### Azure Deployment

```yaml
# azure-pipelines.yml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: DotNetCoreCLI@2
  displayName: 'Build'
  inputs:
    command: 'build'
    projects: '**/*.csproj'

- task: DotNetCoreCLI@2
  displayName: 'Test'
  inputs:
    command: 'test'
    projects: '**/*Tests/*.csproj'

- task: DotNetCoreCLI@2
  displayName: 'Publish'
  inputs:
    command: 'publish'
    publishWebProjects: true
    arguments: '--configuration Release --output $(Build.ArtifactStagingDirectory)'

- task: AzureWebApp@1
  displayName: 'Deploy to Azure'
  inputs:
    azureSubscription: 'your-subscription'
    appName: 'your-app-name'
    package: '$(Build.ArtifactStagingDirectory)/**/*.zip'
``` -->

### Environment Configuration

#### Production Settings

```json
{
  "ConnectionStrings": {
    "SqlDb": "Server=tcp:prod-server.database.windows.net,1433;Initial Catalog=MyIdProd;User ID=admin;Password=${DB_PASSWORD};"
  },
  "Identity": {
    "JwtExpirationMinutes": 60,
    "FromAddress": "no-reply@yourcompany.com",
    "MntcRoute": "https://yourapp.com/admin",
    "CustomerRoute": "https://yourapp.com/accounts"
  },
  "SendGridSettings": {
    "ApiKey": "${SENDGRID_API_KEY}"
  }
}
```

## 🔧 Troubleshooting

### Common Issues

#### Database Connection Issues

```bash
# Check connection string
dotnet ef dbcontext info --project Identity/Infrastructure/ID.Infrastructure

# Update database
dotnet ef database update --project Identity/Infrastructure/ID.Infrastructure

# Reset database (development only)
dotnet ef database drop --project Identity/Infrastructure/ID.Infrastructure
dotnet ef database update --project Identity/Infrastructure/ID.Infrastructure
```

#### JWT Token Issues

```csharp
// Verify token configuration
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(GetRSAKey()),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });
}
```

#### Migration Issues

```bash
# Add new migration
dotnet ef migrations add MigrationName --project Identity/Infrastructure/ID.Infrastructure

# Remove last migration
dotnet ef migrations remove --project Identity/Infrastructure/ID.Infrastructure

# Generate SQL script
dotnet ef migrations script --project Identity/Infrastructure/ID.Infrastructure
```

### Performance Issues

#### Entity Framework Optimization

```csharp
// Use projections for read operations
var teams = await context.Teams
    .Where(t => t.IsActive)
    .Select(t => new TeamSummaryDto
    {
        Id = t.Id.Value,
        Name = t.TeamName,
        MemberCount = t.Members.Count
    })
    .ToListAsync();

// Use AsNoTracking for read-only queries
var team = await context.Teams
    .AsNoTracking()
    .FirstOrDefaultAsync(t => t.Id == teamId);
```



## 🛠️ Development Guidelines

### Architecture Principles

1. **Clean Architecture**: Clear separation of concerns
2. **Domain-Driven Design**: Rich domain model with business logic
3. **CQRS Pattern**: Separate read and write operations
4. **Event-Driven**: Loose coupling through domain events
5. **Testability**: Comprehensive test coverage

### Coding Standards

```csharp
// Use explicit naming and strong typing
public class CreateTeamCommand : ICommand<TeamId>
{
    public Name TeamName { get; }
    public Description TeamDescription { get; }
    
    public CreateTeamCommand(Name teamName, Description teamDescription)
    {
        TeamName = teamName ?? throw new ArgumentNullException(nameof(teamName));
        TeamDescription = teamDescription ?? throw new ArgumentNullException(nameof(teamDescription));
    }
}

// Follow async/await patterns
public async Task<Result<TeamId>> Handle(CreateTeamCommand command, CancellationToken cancellationToken)
{
    var team = Team.Create(command.TeamName, command.TeamDescription, _validator);
    await _teamRepository.AddAsync(team);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    return Result.Success(team.Id);
}
```

### Testing Guidelines

```csharp
// Arrange-Act-Assert pattern
[Fact]
public async Task CreateTeam_WithValidData_ShouldCreateTeam()
{
    // Arrange
    var command = new CreateTeamCommand(
        new Name("Test Team"),
        new Description("Test Description"));
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeNull();
}

// Use meaningful test names
[Theory]
[InlineData("")]
[InlineData(" ")]
[InlineData(null)]
public async Task CreateTeam_WithInvalidName_ShouldReturnValidationError(string invalidName)
{
    // Test implementation
}
```

## 📖 Additional Resources

### Documentation
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

### Related Projects
- [ClArch.SimpleSpecification](../Libs/ClArch.SimpleSpecification/README.md) - Specification pattern implementation
- [ClArch.ValueObjects](../Libs/ClArch.ValueObjects/ReadMe.md) - Value object library
- [TeamManagerService Refactoring](TeamManagerServiceRefactoring.md) - Service architecture improvements

### Community
- [.NET Community](https://dotnet.microsoft.com/platform/community)
- [Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
- [Entity Framework Core](https://github.com/dotnet/efcore)

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Setup

```bash
# Clone the repository
git clone <repository-url>
cd MyId

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Start the demo application
dotnet run --project Apps/MyIdDemo
```

---

## 📞 Support

For support, questions, or feature requests:

- Create an issue in the repository
- Check the [troubleshooting guide](#🔧-troubleshooting)
- Review the [API documentation](#🌐-api-documentation)

---

**Happy coding! 🚀**
