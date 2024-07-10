using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gateway.WebAPI;

public class AuthDBContext(DbContextOptions<AuthDBContext> options) : IdentityDbContext<AuthUser>(options)
{
}
