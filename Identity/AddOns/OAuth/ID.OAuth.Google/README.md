# Google OAuth Integration

This package provides secure server-side Google OAuth authentication for the MyId Identity system using Google's official JWT verification library.

## 🔐 Security Features

- **Server-side JWT verification** - Uses Google's official `Google.Apis.Auth` library
- **No client-provided identity claims** - All user data comes from Google's verified JWT payload
- **Automatic signature validation** - Google's library handles all cryptographic verification
- **Comprehensive validation** - Issuer, audience, expiration, and algorithm verification
- **Zero trust client model** - Only the raw JWT token is accepted from clients

## 📦 Setup

### 1. Configure Google OAuth

1. Create a project in the [Google Cloud Console](https://console.cloud.google.com/)
2. Enable the Google+ API or People API
3. Create OAuth 2.0 credentials (Web application)
4. Configure authorized JavaScript origins and redirect URIs
5. Note your Client ID

### 2. Configure Services

```csharp
// In Program.cs or Startup.cs
builder.Services.AddGoogleOAuth(builder.Configuration);

// Or with explicit configuration
builder.Services.AddGoogleOAuth(options =>
{
    options.ClientId = "your-google-client-id.apps.googleusercontent.com";
});
```

### 3. Configuration

```json
{
  "GoogleOAuth": {
    "ClientId": "your-google-client-id.apps.googleusercontent.com"
  }
}
```

## 🚀 Usage

### Client-Side Flow

1. **Client obtains Google ID token** using Google's JavaScript library (`gapi` or `@google-cloud/local-auth`)
2. **Client sends raw JWT token** to your API via the GoogleSignInDto
3. **Server verifies JWT** using Google's official library and creates/authenticates user

### Server-Side Handler

```csharp
[HttpPost("google-signin")]
public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDto dto)
{
    var command = new GoogleSignInCmd(dto);
    var result = await _mediator.Send(command);
    
    if (result.Succeeded)
        return Ok(result.Value); // Returns JwtPackage
    
    return Unauthorized(result.ErrorMessage);
}
```

### DTO Structure (Clean & Secure)

```csharp
public class GoogleSignInDto
{
    public string GoogleToken { get; set; }     // Required - Raw JWT from Google
    public Guid? SubscriptionId { get; set; }  // Optional - Your business data  
    public string? DeviceId { get; set; }      // Optional - Device tracking
    
    // ✅ NO identity claims - all come from verified JWT!
}
```

## 🔒 Security Model

### What Gets Verified (Automatically by Google's Library)

- ✅ **JWT Signature** - Cryptographically verified against Google's public keys
- ✅ **Issuer validation** - Ensures token comes from `accounts.google.com`
- ✅ **Audience validation** - Verifies token was issued for your Client ID
- ✅ **Expiration validation** - Checks token hasn't expired
- ✅ **Algorithm validation** - Ensures RS256 signature algorithm
- ✅ **Token format** - Validates JWT structure and length limits

### What's Trusted

- ✅ **GoogleVerifiedPayload** - All claims from Google's verified JWT
- ✅ **SubscriptionId** - Your server-generated business data
- ✅ **DeviceId** - Client device identifier (for tracking/audit)

### What's Rejected

- ❌ **Client-provided identity claims** - Email, name, etc. from client are ignored
- ❌ **Invalid JWTs** - Malformed, expired, or tampered tokens
- ❌ **Wrong audience** - Tokens not issued for your Client ID
- ❌ **Unsigned tokens** - Tokens without valid Google signatures

## 📊 Token Verification Process

```
1. Client → Your API: Raw Google JWT token
2. Your API → Google's Library: GoogleJsonWebSignature.ValidateAsync()
3. Google's Library → Google: Fetch current public keys (cached)
4. Google's Library → Your API: Verified JWT payload or exception
5. Your API → Client: JWT package or error
```

## 🔧 Key Components

### 1. GoogleTokenVerifier
```csharp
public async Task<GenResult<GoogleVerifiedPayload>> VerifyTokenAsync(string token, CancellationToken cancellationToken)
{
    var payload = await GoogleJsonWebSignature.ValidateAsync(
        jwt: token,
        validationSettings: new GoogleJsonWebSignature.ValidationSettings { 
            Audience = [_options.ClientId],
        });
    
    return GenResult<GoogleVerifiedPayload>.Success(new GoogleVerifiedPayload(payload));
}
```

### 2. GoogleSignInHandler
- Verifies Google JWT tokens server-side
- Finds existing users or creates new ones using **only** verified data
- Integrates with your `IJwtPackageProvider` for JWT creation
- Handles 2FA requirements appropriately

### 3. GoogleVerifiedPayload
- Strongly-typed wrapper around Google's JWT payload
- Contains verified user identity data (email, name, picture, etc.)
- All data guaranteed to come from Google's servers

## 🧪 Testing

### Comprehensive Test Coverage

- **Token verification tests** - Invalid tokens, malformed JWTs, wrong audience
- **Handler integration tests** - Full sign-in flow testing
- **Error handling tests** - Network failures, invalid responses
- **Security tests** - Ensures only verified data is used
- **Validation tests** - DTO and command validation

### Test Structure
```
ID.OAuth.Google.Tests/
├── Auth/
│   └── GoogleTokenVerifierTests.cs
├── Handler/
│   └── GoogleSignInHandlerTests.cs
└── TestData/
    └── GoogleSignInDtoFactory.cs
```

## ⚡ Performance & Security Benefits

### Google's Official Library Advantages
- **Automatic key rotation** - Google's library handles certificate updates
- **Efficient validation** - Optimized cryptographic operations
- **Built-in caching** - Public keys cached automatically
- **Battle-tested** - Used by millions of applications
- **Regular updates** - Maintained by Google's security team

### Zero Network Calls Required
- **Offline verification** - No API calls to Google required
- **High performance** - Local JWT validation
- **High availability** - No dependency on Google's API uptime
- **Cost effective** - No API usage limits or costs

## 🔄 Architecture Comparison

| Component | Google OAuth | Traditional OAuth |
|-----------|--------------|-------------------|
| Verification | Local JWT validation | Remote API calls |
| Performance | Very fast (offline) | Slower (network dependent) |
| Dependencies | Google.Apis.Auth | HttpClient + custom logic |
| Security | Google's crypto library | Custom implementation |
| Maintenance | Google handles updates | Manual security updates |

## 📝 Google JWT Claims

The verified payload includes standard and Google-specific claims:

```csharp
public class GoogleVerifiedPayload
{
    public string Email { get; }          // Verified email address
    public bool EmailVerified { get; }    // Email verification status
    public string GivenName { get; }      // First name
    public string FamilyName { get; }     // Last name  
    public string Name { get; }           // Full name
    public string Picture { get; }        // Profile picture URL
    public string Locale { get; }         // User's locale
    public string Issuer { get; }         // Token issuer (accounts.google.com)
    public string Subject { get; }        // Google user ID
    // ... additional claims
}
```

## 🔧 Dependencies

- `Google.Apis.Auth` - Google's official JWT verification library
- `MediatR` - Command/query handling
- `Microsoft.AspNetCore.Authentication.Google` - ASP.NET Core integration
- `MyResults` - Result type patterns
- `LoggingHelpers` - Enhanced logging

## 🛡️ Security Best Practices Implemented

### 1. No Client Trust
```csharp
// ❌ NEVER do this - trusting client claims
// var email = dto.Email; // Could be forged!

// ✅ ALWAYS do this - use verified claims only
var email = verifiedPayload.Email; // From Google's verified JWT
```

### 2. Server-Side Verification Only
```csharp
// ✅ Verify every token server-side
var verifyResult = await _verifier.VerifyTokenAsync(dto.GoogleToken, cancellationToken);
if (!verifyResult.Succeeded)
    return verifyResult.Convert<JwtPackage>(); // Reject invalid tokens
```

### 3. Explicit Parameter Validation
```csharp
// ✅ Force explicit user/team validation in handlers
public async Task<JwtPackage> RefreshJwtPackageAsync(
    IdRefreshToken existingToken,
    AppUser user,    // ✅ Explicitly required
    Team team,       // ✅ Explicitly required  
    string? currentDeviceId = null)
```

## 🚀 Integration with MyId Identity System

This Google OAuth implementation seamlessly integrates with:

- **JWT Package Provider** - Uses your smart refresh token logic
- **User Registration Service** - Creates users with verified Google data
- **Team Management** - Respects team-based authentication
- **Two-Factor Authentication** - Properly handles 2FA requirements
- **Refresh Token Policies** - Supports all refresh update strategies

## 📋 Example Configuration

```json
{
  "GoogleOAuth": {
    "ClientId": "123456789-abcdef.apps.googleusercontent.com"
  },
  "JwtOptions": {
    "RefreshTokenUpdatePolicy": "ThreeQuarterLife",
    "TokenExpirationMinutes": 60
  }
}
```

The Google OAuth integration provides **enterprise-grade security** with minimal configuration and maximum performance through Google's battle-tested JWT verification library. 🔐✅
