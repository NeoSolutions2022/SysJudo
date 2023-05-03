using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class GrupoAcessoMapping : IEntityTypeConfiguration<GrupoAcesso>
{
    public void Configure(EntityTypeBuilder<GrupoAcesso> builder)
    {
        builder
            .Property(c => c.Nome)
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .Property(c => c.Descricao)
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(e => e.Administrador)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder
            .HasOne(c => c.Cliente)
            .WithMany(c => c.GrupoAcessos)
            .HasForeignKey(c => c.ClienteId)
            .HasPrincipalKey(c => c.Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}