using Microsoft.EntityFrameworkCore;
using SysJudo.Core.Authorization;

namespace SysJudo.Infra.Context;

public sealed class ApplicationDbContext : BaseApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IAuthenticatedUser authenticatedUser) : base(options, authenticatedUser)
    {
    }
}