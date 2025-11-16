# Facebook OAuth Integration

This package provides secure server-side Facebook OAuth authentication for the MyId Identity system.

## 🔐 Security Features

- **Server-side token verification** - All Facebook access tokens are validated against Facebook's API
- **No client-provided identity claims** - All user data comes from Facebook's verified API responses
- **Two-step verification process** - Token validation + user profile retrieval
- **Comprehensive error handling** - Proper logging and error propagation

## 📦 Setup

### 1. Configure Facebook App

1. Create a Facebook App in the [Facebook Developer Console](https://developers.facebook.com/)
2. Configure OAuth redirect URIs for your client application
3. Note your App ID and App Secret

### 2. Configure Services

```csharp
// In Program.cs or Startup.cs
builder.Services.AddFacebookOAuth(builder.Configuration);

// Or with explicit configuration
builder.Services.AddFacebookOAuth(options =>
{
    options.AppId = "your-facebook-app-id";
    options.AppSecret = "your-facebook-app-secret";
    options.GraphApiVersion = "v18.0";
});
```

> **Note**: This implementation uses `Microsoft.Extensions.Http.Resilience` instead of the deprecated `Polly.Extensions.Http` for modern resilience patterns in .NET 8+.

### 3. Configuration

```json
{
  "FacebookOAuth": {
    "AppId": "your-facebook-app-id",
    "AppSecret": "your-facebook-app-secret", 
    "GraphApiVersion": "v18.0",
    "GraphApiBaseUrl": "https://graph.facebook.com",
    "RequestTimeoutSeconds": 30
  }
}
```

## 🚀 Usage

### Client-Side Flow

1. **Client obtains Facebook access token** using Facebook's JavaScript SDK
2. **Client sends token to your API** via the FacebookSignInDto
3. **Server verifies token** and creates/authenticates user

### Server-Side Handler

```csharp
[HttpPost("facebook-signin")]
public async Task<IActionResult> FacebookSignIn([FromBody] FacebookSignInDto dto)
{
    var command = new FacebookSignInCmd(dto);
    var result = await _mediator.Send(command);
    
    if (result.Succeeded)
        return Ok(result.Value); // Returns JwtPackage
    
    return Unauthorized(result.ErrorMessage);
}
```

### DTO Structure

```csharp
public class FacebookSignInDto
{
    public string FacebookAccessToken { get; set; }  // Required - from Facebook OAuth
    public Guid? SubscriptionId { get; set; }        // Optional - your business data
    public string? DeviceId { get; set; }            // Optional - device tracking
}
```

## 🔒 Security Model

### What Gets Verified

- ✅ **Access token authenticity** - Validated against Facebook's debug_token endpoint
- ✅ **App ownership** - Ensures token belongs to your Facebook app
- ✅ **Token expiration** - Checks if token is still valid
- ✅ **User profile data** - Retrieved from Facebook's Graph API

### What's Trusted

- ✅ **FacebookVerifiedPayload** - All data comes from Facebook's servers
- ✅ **SubscriptionId** - Your server-generated business data
- ✅ **DeviceId** - Client device identifier (for tracking)

### What's Rejected

- ❌ **Client-provided identity claims** - Email, name, etc. from client are ignored
- ❌ **Unverified tokens** - Invalid or expired tokens are rejected
- ❌ **Wrong app tokens** - Tokens not belonging to your app are rejected

## 📊 Token Verification Process

```
1. Client → Your API: FacebookAccessToken
2. Your API → Facebook: Validate token (debug_token endpoint)
3. Facebook → Your API: Token validation result
4. Your API → Facebook: Get user profile (Graph API)
5. Facebook → Your API: Verified user data
6. Your API → Client: JWT package or error
```

## 🧪 Testing

The package includes comprehensive test coverage:

- **Token verification tests** - Mock Facebook API responses
- **Handler integration tests** - End-to-end flow testing
- **Error handling tests** - Network failures, invalid tokens, etc.
- **Security tests** - Ensure only verified data is used

## 🔧 Dependencies

- `Microsoft.Extensions.Http` - For Facebook API calls
- `Microsoft.Extensions.Options` - Configuration management
- `Microsoft.Extensions.Logging` - Structured logging
- `System.Text.Json` - JSON serialization
- `MyResults` - Result type patterns
- `LoggingHelpers` - Enhanced logging

## 📝 Facebook Graph API

This integration uses Facebook's Graph API v18.0 by default:

- **Token Debug**: `/debug_token` - Validates access tokens
- **User Profile**: `/me` - Retrieves verified user data

### Required Facebook Permissions

- `email` - Access to user's email address
- `public_profile` - Access to public profile information

## ⚡ Performance & Resilience

- **HTTP connection pooling** via `IHttpClientFactory`
- **Named HttpClient** with built-in resilience patterns
- **Automatic retries** with exponential backoff (3 attempts)
- **Circuit breaker** protection against cascading failures
- **Configurable timeouts** for Facebook API calls
- **Efficient JSON deseriization** with System.Text.Json
- **Structured logging** for monitoring and debugging

### Resilience Features

The Facebook OAuth implementation uses `Microsoft.Extensions.Http.Resilience` for robust HTTP communication:

- **Retry Strategy**: 3 attempts with exponential backoff starting at 1 second
- **Circuit Breaker**: Opens after 50% failure rate (minimum 3 requests), stays open for 30 seconds
- **Timeouts**: 10 seconds per attempt, 30 seconds total request timeout
- **Automatic Recovery**: Circuit breaker automatically closes when conditions improve

## 🔄 Comparison to Google OAuth

| Feature | Google OAuth | Facebook OAuth |
|---------|--------------|----------------|
| Token Type | JWT (server verification) | Access Token (API verification) |
| Verification | Local JWT validation | Remote API calls |
| Official SDK | ✅ Google.Apis.Auth | ❌ Use HttpClient |
| Performance | Faster (local) | Slower (API calls) |
| Security | JWT signatures | API validation |

Both implementations follow the same secure patterns and provide equivalent security guarantees.
