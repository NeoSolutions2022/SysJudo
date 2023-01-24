using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ScottBrady91.AspNetCore.Identity;
using SysJudo.Application.Contracts;
using SysJudo.Application.Notifications;
using SysJudo.Application.Services;
using SysJudo.Core.Enums;
using SysJudo.Core.Extension;
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
        services.AddScoped<ICidadeService, CidadeService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IEstadoService, EstadoService>();
        services.AddScoped<IFaixaService, FaixaService>();
        services.AddScoped<IPaisService, PaisService>();
        services.AddScoped<IRegiaoService, RegiaoService>();
        services.AddScoped<ISistemaService, SistemaService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAtletaService, AtletaService>();
        services.AddScoped<IEmissoresIdentidadeService, EmissoresIdentidadeService>();
        services.AddScoped<INacionalidadeService, NacionalidadeService>();
        services.AddScoped<IProfissaoService, ProfissaoService>();
        services.AddScoped<IFileService, FileService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdministradorAuthService, AdministradorAuthService>();
        services.AddScoped<IPasswordHasher<Usuario>, Argon2PasswordHasher<Usuario>>();
        services.AddScoped<IPasswordHasher<Administrador>, Argon2PasswordHasher<Administrador>>();
        services.AddScoped<INotificator, Notificator>();
    }

    public static void UseStaticFileConfiguration(this IApplicationBuilder app, IConfiguration configuration)
    {
        var uploadSettings = configuration.GetSection("UploadSettings");
        var publicBasePath = uploadSettings.GetValue<string>("PublicBasePath");
        var privateBasePath = uploadSettings.GetValue<string>("PrivateBasePath");

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(publicBasePath),
            RequestPath = $"/{EPathAccess.Public.ToDescriptionString()}"
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(privateBasePath),
            RequestPath = $"/{EPathAccess.Private.ToDescriptionString()}",
            OnPrepareResponse = ctx =>
            {
                if (ctx.Context.User.UsuarioAutenticado()) return;

                // respond HTTP 401 Unauthorized.
                ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                ctx.Context.Response.ContentLength = 0;
                ctx.Context.Response.Body = Stream.Null;
                ctx.Context.Response.Headers.Add("Cache-Control", "no-store");
            }
        });
    }
}