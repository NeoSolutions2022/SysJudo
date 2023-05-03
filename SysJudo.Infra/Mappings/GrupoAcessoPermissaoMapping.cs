using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class GrupoAcessoPermissaoMapping : IEntityTypeConfiguration<GrupoAcessoPermissao>
{
    public void Configure(EntityTypeBuilder<GrupoAcessoPermissao> builder)
    {
        builder
            .HasKey(gap => new { gap.GrupoAcessoId, gap.PermissaoId });

        builder
            .Property(e => e.Tipo)
            .HasMaxLength(4)
            .IsRequired();
        
        builder
            .HasOne(gap => gap.Permissao)
            .WithMany(p => p.Grupos)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(gap => gap.GrupoAcesso)
            .WithMany(ga => ga.Permissoes)
            .OnDelete(DeleteBehavior.Restrict);
    }
}