using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace ID.Infrastructure.Models;
public record MyIdBuilders(
    IdentityBuilder IdentityBuilder,
    AuthenticationBuilder AuthenticationBuilder
);
