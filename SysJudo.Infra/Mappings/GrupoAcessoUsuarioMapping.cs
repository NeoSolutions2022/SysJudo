using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class GrupoAcessoUsuarioMapping : IEntityTypeConfiguration<GrupoAcessoUsuario>
{
    public void Configure(EntityTypeBuilder<GrupoAcessoUsuario> builder)
    {
        builder
            .HasKey(gap => new { gap.GrupoAcessoId, gap.UsuarioId });

        builder
            .HasOne(gap => gap.Usuario)
            .WithMany(p => p.GrupoAcessos)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(gap => gap.GrupoAcesso)
            .WithMany(ga => ga.Usuarios)
            .OnDelete(DeleteBehavior.Restrict);
    }
}