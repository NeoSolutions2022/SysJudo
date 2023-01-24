using Microsoft.EntityFrameworkCore;
using SysJudo.Core.Authorization;
using SysJudo.Domain.Contracts;
using SysJudo.Infra.Extensions;

namespace SysJudo.Infra.Context;

public sealed class TennantApplicationDbContext : BaseApplicationDbContext
{
    public TennantApplicationDbContext(DbContextOptions<TennantApplicationDbContext> options, IAuthenticatedUser authenticatedUser) : base(options, authenticatedUser)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyGlobalFilters<ITenant>(t => t.ClienteId == AuthenticatedUser.ClienteId);
        base.OnModelCreating(modelBuilder);
    }
}