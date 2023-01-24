﻿using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using SysJudo.Application.Settings;
using SysJudo.Core.Settings;

namespace SysJudo.Api.Configuration;

public static class AuthenticationConfiguration
{
    public static void AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors();
        services.AddControllers();

        var key = Encoding.ASCII.GetBytes(Settings.Secret);
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        
        // services.AddAuthorization();

        // services
        //     .AddDataProtection()
        //     .PersistKeysToFileSystem(new DirectoryInfo(appSettings.CaminhoKeys));

        // services
        //     .AddJwksManager()
        //     .UseJwtValidation();
        //
        // services.AddMemoryCache();
        // services.AddHttpContextAccessor();
    }
    
    public static void UseAuthenticationConfig(this IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
    }
}