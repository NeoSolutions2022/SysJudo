using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SysJudo.Core.Authorization;
using SysJudo.Core.Extension;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;
using SysJudo.Infra.Context;
using SysJudo.Infra.Repositories;
using SysJudo.Infra.Repositories.RepositoriesFiltros;

namespace SysJudo.Infra;

public static class DependencyInjection
{
    public static void ConfigureDataBase(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddHttpContextAccessor();

        serviceCollection.AddScoped<IAuthenticatedUser>(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

            return httpContextAccessor.UsuarioAutenticado()
                ? new AuthenticatedUser(httpContextAccessor)
                : new AuthenticatedUser();
        });

        serviceCollection.AddDbContext<ApplicationDbContext>(optionsAction =>
        {
            optionsAction.UseSqlServer(configuration.GetConnectionString("Default"));
            optionsAction.EnableDetailedErrors();
            optionsAction.EnableSensitiveDataLogging();
        });
        serviceCollection.AddDbContext<TennantApplicationDbContext>(optionsAction =>
        {
            optionsAction.UseSqlServer(configuration.GetConnectionString("Default"));
            optionsAction.EnableDetailedErrors();
            optionsAction.EnableSensitiveDataLogging();
        });

        serviceCollection.AddScoped<BaseApplicationDbContext>(serviceProvider =>
        {
            var autenticatedUser = serviceProvider.GetRequiredService<IAuthenticatedUser>();
            if (autenticatedUser.UsuarioLogado)
            {
                return serviceProvider.GetRequiredService<TennantApplicationDbContext>();
            }

            return serviceProvider.GetRequiredService<ApplicationDbContext>();
        });
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAdministradorRepository, AdministradorRepository>();
        services.AddScoped<IAgremiacaoRepository, AgremiacaoRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IFaixaRepository, FaixaRepository>();
        services.AddScoped<IRegiaoRepository, RegiaoRepository>();
        services.AddScoped<ISistemaRepository, SistemaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAtletaRepository, AtletaRepository>();
        services.AddScoped<IGrupoAcessoRepository, GrupoAcessoRepository>();
        services.AddScoped<IPermissaoRepository, PermissaoRepository>();
        services.AddScoped<IEmissoresIdentidadeRepository, EmissoresIdentidadeRepository>();
        services.AddScoped<INacionalidadeRepositoty, NacionalidadeRepository>();
        services.AddScoped<IProfissaoRepository, ProfissaoRepository>();
        services.AddScoped<IRegistroDeEventoRepository, RegistroDeEventosRepository>();

        #region Filtro

        services.AddScoped<IAgremiacaoFiltroRepository, AgremiacaoFiltroRepository>();

        #endregion
    }

    public static void UseMigrations(this IApplicationBuilder app, IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}