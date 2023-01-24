using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class SistemaMapping : IEntityTypeConfiguration<Sistema>
{
    public void Configure(EntityTypeBuilder<Sistema> builder)
    {
        builder.Property(s => s.Sigla)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(s => s.Descricao)
            .IsRequired()
            .HasMaxLength(60);
        
        builder.Property(s => s.Versao)
            .IsRequired()
            .HasMaxLength(30);
    }
}