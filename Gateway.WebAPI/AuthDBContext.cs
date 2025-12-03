using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gateway.WebAPI;

/// Summary:
/// AuthDBContext provides the EF Core Identity DbContext used by the gateway to persist users and roles.
/// It inherits from IdentityDbContext<AuthUser> which configures all the Identity tables (AspNetUsers, AspNetRoles, etc.).
/// This DbContext is registered in the Program.cs with AddDbContext so Identity can store and retrieve user information.
public class AuthDBContext(DbContextOptions<AuthDBContext> options) : IdentityDbContext<AuthUser>(options)
{
}
