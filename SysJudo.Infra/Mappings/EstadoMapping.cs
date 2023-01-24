using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class EstadoMapping : IEntityTypeConfiguration<Estado>
{
    public void Configure(EntityTypeBuilder<Estado> builder)
    {
        builder.Property(e => e.Descricao)
            .HasMaxLength(60)
            .IsUnicode(false);

        builder.Property(e => e.Sigla)
            .HasMaxLength(2)
            .IsUnicode(false);

        builder
            .HasOne(d => d.Pais)
            .WithMany(p => p.Estados)
            .HasForeignKey(d => d.IdPais)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(d => d.Cliente)
            .WithMany(p => p.Estados)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}