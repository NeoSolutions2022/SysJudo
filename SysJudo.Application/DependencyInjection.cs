using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottBrady91.AspNetCore.Identity;
using SysJudo.Application.Contracts;
using SysJudo.Application.Notifications;
using SysJudo.Application.Services;
using SysJudo.Core.Settings;
using SysJudo.Domain.Entities;
using SysJudo.Infra;

namespace SysJudo.Application;

public static class DependencyInjection
{
    public static void ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<UploadSettings>(configuration.GetSection("UploadSettings"));

        services.ConfigureDataBase(configuration);
        services.ConfigureRepositories();

        services
            .AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IAdministradorService, AdministradorService>();
        services.AddScoped<IAgremiacaoService, AgremiacaoService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IFaixaService, FaixaService>();
        services.AddScoped<IRegiaoService, RegiaoService>();
        services.AddScoped<ISistemaService, SistemaService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAtletaService, AtletaService>();
        services.AddScoped<IEmissoresIdentidadeService, EmissoresIdentidadeService>();
        services.AddScoped<INacionalidadeService, NacionalidadeService>();
        services.AddScoped<IProfissaoService, ProfissaoService>();
        services.AddScoped<IRegistroDeEventoService, RegistroDeEventoService>();
        services.AddScoped<IFileService, FileService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdministradorAuthService, AdministradorAuthService>();
        services.AddScoped<IGrupoAcessoService, GrupoAcessoService>();
        services.AddScoped<IPasswordHasher<Usuario>, Argon2PasswordHasher<Usuario>>();
        services.AddScoped<IPasswordHasher<Administrador>, Argon2PasswordHasher<Administrador>>();
        services.AddScoped<INotificator, Notificator>();
    }
}