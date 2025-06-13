# Authenticators

Contains all Authenticators in the form of ActionFilters, ResourceFilters, and Policies wrapped in container classes. These provide fine-grained, team/role-based authorization for controller actions and resources.

## Available Filter Types

Each authenticator provides three types of authorization mechanisms:

- **ActionFilter**: Applied to controller actions, executes during action execution
- **ResourceFilter**: Applied to controller actions, executes earlier in the pipeline (before model binding)
- **Policy**: Used with `[Authorize(Policy = "PolicyName")]` attribute for declarative authorization

## Usage Examples

### Basic Action Authorization
```csharp
[HttpPost]
[CustomerAuthenticator.ActionFilter]
public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] TeamDto dto)
{
    // Only customer team members can access this action
}
```

### Position-Based Authorization
```csharp
[HttpDelete("{id}")]
[MntcMinimumAuthenticator.ActionFilter]
public async Task<ActionResult> DeleteResource(Guid id)
{
    // Only maintenance team members with minimum position can access
}
```

### Resource Filter with Position Level
```csharp
[HttpGet]
[MntcMinimumAuthenticator.ResourceFilter(1)]
public async Task<ActionResult<TeamDto>> GetAllTeams()
{
    // Maintenance team members with position level 1 or higher
}
```

### Policy-Based Authorization
```csharp
[HttpPatch]
[Authorize(Policy = CustomerAuthenticator.Policy.Name)]
public async Task<ActionResult> UpdateResource()
{
    // Uses policy for authorization
}
```

## Available Authenticators

### Team-Based Authenticators
- `CustomerAuthenticator` - Customer team members only
- `MntcAuthenticator` - Maintenance team members only  
- `SuperAuthenticator` - Super team members only

### Position-Based Authenticators
- `CustomerMinimumAuthenticator` - Customer team with minimum position
- `MntcMinimumAuthenticator` - Maintenance team with minimum position
- `SuperMinimumAuthenticator` - Super team with minimum position
- `PositionMinimumAuthenticator` - Any team with minimum position

### Leader Authenticators
- `CustomerLeaderAuthenticator` - Customer team leaders
- `MntcLeaderAuthenticator` - Maintenance team leaders
- `SuperLeaderAuthenticator` - Super team leaders
- `LeaderAuthenticator` - Any team leaders

### Special Combinations
- `SuperMinimumOrDevAuthenticator` - Super minimum or development access
- `MntcMinimumOrDevAuthenticator` - Maintenance minimum or development access

## Authorization Patterns

### Exact Position vs Hierarchical
- **Exact Position**: Uses `AAuthExactPositionHandler` - requires exact position match
- **Hierarchical**: Uses `AAuthPositionHandler` - allows position level or higher

### Controller Usage Patterns
From real controller examples:
```csharp
// IdTeamsController.cs
[SuperMinimumOrDevAuthenticator.ActionFilter]  // Team creation
[LeaderAuthenticator.ActionFilter]             // Team editing/deletion
[MntcMinimumAuthenticator.ResourceFilter(1)]   // Team viewing with position

// UserManagementController.cs  
[MntcMinimumAuthenticator.ActionFilter]        // User management operations
```

## Generating New Authenticators

To create new custom authenticators, use the Visual Studio templates located in `Identity/Extras/Templates/`:

### Available Templates

1. **CustomAuth** - Basic team-based authenticator
   - Path: `Extras/Templates/CustomAuth/`
   - Use for: Simple team membership validation

2. **CustomPositionAuth** - Position-based authenticator  
   - Path: `Extras/Templates/CustomPositionAuth/`
   - Use for: Team with position level requirements

3. **CustomExactPositionAuth** - Exact position authenticator
   - Path: `Extras/Templates/CustomExactPositionAuth/`  
   - Use for: Strict position matching (no hierarchical override)

### Installing Templates (Visual Studio)

1. Copy the template folder to your Visual Studio templates directory:
   - `%USERPROFILE%\Documents\Visual Studio 2022\Templates\ItemTemplates\`
2. Restart Visual Studio
3. Right-click project → Add → New Item → Search for "CustomAuth"
4. Select appropriate template and customize the generated code

### Template Customization

After generating from template:
1. Update the `ExtraAuthorization` method with your specific logic
2. Modify team/position validation as needed
3. Update XML documentation
4. Add to appropriate namespace

## Implementation Notes

- All authenticators inherit from base classes in `ID.Application.Authenticators.Abstractions`
- Position levels are typically 1-10 (configurable via global settings)
- Unauthenticated users receive `UnauthorizedResult` (401)
- Unauthorized users receive `ForbidResult` (403)
- Use ResourceFilter for early pipeline execution (before model binding)
- Use ActionFilter for standard action-level authorization

## Related Files

- Base classes: `ID.Application.Authenticators.Abstractions/`
- Extension methods: `ID.Application.Utility.ExtensionMethods/`
- Templates: `Identity/Extras/Templates/CustomAuth*/`
- Controller examples: `ID.Presentation.Controllers/IdTeamsController.cs`

## Testing Authenticators

### Demo Controllers

Demo controllers are available in `Apps/MyIdDemo/Controllers/` to test all authenticators:

- **SuperAuthenticatorDemoController** - `/api/SuperAuthenticatorDemo`
- **MaintenanceAuthenticatorDemoController** - `/api/MaintenanceAuthenticatorDemo`  
- **CustomerAuthenticatorDemoController** - `/api/CustomerAuthenticatorDemo`
- **MiscellaneousAuthenticatorDemoController** - `/api/MiscellaneousAuthenticatorDemo`
- **AuthenticatorDemoController** - `/api/AuthenticatorDemo` (endpoint listing)

### Testing Workflow

1. **Get endpoint list**: `GET /api/AuthenticatorDemo/endpoints`
2. **Authenticate** with JWT token containing appropriate team/position claims
3. **Test endpoints** to verify authenticator behavior
4. **Check responses**:
   - `200 OK` - Authentication passed
   - `401 Unauthorized` - Not authenticated  
   - `403 Forbidden` - Authenticated but insufficient permissions

### Example Test Scenarios

```bash
# Test customer authenticator (requires customer team JWT)
GET /api/CustomerAuthenticatorDemo/customer
Response: {"message":"You are authenticated!!!","Authenticator":"CustomerAuthenticator"}

# Test position-based authenticator (requires position level 2+)
GET /api/MiscellaneousAuthenticatorDemo/position-minimum-resource-filter
Response: {"message":"You are authenticated!!!","Authenticator":"PositionMinimumAuthenticator.ResourceFilter(2)"}

# Test policy-based authenticator
GET /api/MiscellaneousAuthenticatorDemo/policy-example
Response: {"message":"You are authenticated!!!","Authenticator":"Policy: MntcAuthenticator.Policy"}
```

### Testing with Different JWT Claims

Test with JWT tokens containing different team/position combinations:

- **Customer Team Member** (position 1-5)
- **Maintenance Team Leader** (position 8-10)  
- **Super Team Admin** (position 10)
- **Development Access** (special dev claims)

### Demo Endpoint Categories

#### Super Authenticators (`/api/SuperAuthenticatorDemo`)
- `/super` - Basic super team access
- `/super-minimum` - Super team with minimum position
- `/super-leader` - Super team leadership
- `/super-minimum-or-dev` - Super minimum or development access
- `/super-resource-filter` - Resource filter with position level 1

#### Maintenance Authenticators (`/api/MaintenanceAuthenticatorDemo`)
- `/mntc` - Basic maintenance team access
- `/mntc-minimum` - Maintenance team with minimum position
- `/mntc-leader` - Maintenance team leadership
- `/mntc-leader-minimum` - Maintenance leader with minimum position
- `/mntc-minimum-or-dev` - Maintenance minimum or development access
- `/mntc-resource-filter` - Resource filter with position level 2

#### Customer Authenticators (`/api/CustomerAuthenticatorDemo`)
- `/customer` - Basic customer team access
- `/customer-minimum` - Customer team with minimum position
- `/customer-leader` - Customer team leadership
- `/customer-leader-minimum` - Customer leader with minimum position
- `/customer-resource-filter` - Resource filter with position level 1

#### Miscellaneous Authenticators (`/api/MiscellaneousAuthenticatorDemo`)
- `/leader` - Any team leadership
- `/position-minimum` - Any team with minimum position
- `/position-minimum-with-level` - Specific position level (3+)
- `/position-minimum-resource-filter` - Resource filter with position level 2
- `/leader-resource-filter` - Leadership resource filter
- `/policy-example` - Policy-based authentication example
