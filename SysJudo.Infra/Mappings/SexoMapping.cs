using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class SexoMapping : IEntityTypeConfiguration<Sexo>
{
    public void Configure(EntityTypeBuilder<Sexo> builder)
    {
        builder
            .HasKey(c => c.Id);
            
        builder
            .Property(s => s.Sigla)
            .IsRequired();
        
        builder
            .Property(s => s.Descricao)
            .HasMaxLength(60)
            .IsRequired();
    }
}