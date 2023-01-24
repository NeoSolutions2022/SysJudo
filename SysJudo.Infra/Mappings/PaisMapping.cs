using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class PaisMapping : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Descricao)
            .HasMaxLength(60)
            .IsUnicode(false);

        builder.Property(e => e.Nacionalidade)
            .HasMaxLength(30)
            .IsUnicode(false);

        builder.Property(e => e.Sigla2)
            .HasMaxLength(2)
            .IsUnicode(false);

        builder.Property(e => e.Sigla3)
            .HasMaxLength(3)
            .IsUnicode(false);
        
        builder.HasOne(d => d.Cliente)
            .WithMany(p => p.Paises)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}