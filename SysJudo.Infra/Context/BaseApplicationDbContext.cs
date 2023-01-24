using System.Reflection;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using SysJudo.Core.Authorization;
using SysJudo.Core.Exeptions;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Converters;
using SysJudo.Infra.Extensions;

namespace SysJudo.Infra.Context;

public abstract class BaseApplicationDbContext : DbContext, IUnitOfWork
{
    protected readonly IAuthenticatedUser AuthenticatedUser;
    protected BaseApplicationDbContext(DbContextOptions options, IAuthenticatedUser authenticatedUser) : base(options)
    {
        AuthenticatedUser = authenticatedUser;
    }
    
    public DbSet<Agremiacao> Agremiacoes { get; set; } = null!;
    public DbSet<Sistema> Sistemas { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Faixa> Faixas { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Administrador> Administradores { get; set; } = null!;
    public DbSet<Pais> Paises { get; set; } = null!;
    public DbSet<Estado> Estados { get; set; } = null!;
    public DbSet<Cidade> Cidades { get; set; } = null!;
    public DbSet<Regiao> Regioes { get; set; } = null!;
    public DbSet<EmissoresIdentidade> EmissoresIdentidades { get; set; } = null!;
    public DbSet<Profissao> Profissoes { get; set; } = null!;
    public DbSet<Sexo> Sexos { get; set; } = null!;
    public DbSet<EstadoCivil> EstadosCivis { get; set; } = null!;
    public DbSet<Nacionalidade> Nacionalidades { get; set; } = null!;
    public DbSet<Atleta> Atletas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
            .UseCollation("utf8mb4_0900_ai_ci");
        
        ApplyConfigurations(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (AuthenticatedUser.UsuarioAdministrador) 
            return base.SaveChangesAsync(cancellationToken);

        ApplyTenantChanges();
        
        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Commit() => await SaveChangesAsync() > 0;
    
    private static void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ValidationResult>();

        modelBuilder.ApplyEntityConfiguration();
        modelBuilder.ApplyTenantConfiguration();
    }

    private void ApplyTenantChanges()
    {
        var tenants = ChangeTracker
            .Entries()
            .Where(e => e.Entity is ITenant && e.State is EntityState.Added)
            .ToList();

        if (tenants.Any() && AuthenticatedUser.ClienteId <= 0)
        {
            throw new DomainException("User not defined for tenant entity!");
        }
    
        foreach (var tenant in tenants)
        {
            ((ITenant) tenant.Entity).ClienteId = AuthenticatedUser.ClienteId;
        }
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateOnly>()
            .HaveConversion<DateOnlyCustomConverter>()
            .HaveColumnType("DATE");

        configurationBuilder
            .Properties<TimeOnly>()
            .HaveConversion<TimeOnlyCustomConverter>()
            .HaveColumnType("TIME");
    }
}