using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class NacionalidadeMapping : IEntityTypeConfiguration<Nacionalidade>
{
    public void Configure(EntityTypeBuilder<Nacionalidade> builder)
    {
        builder.Property(c => c.Sigla)
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(c => c.Descricao)
            .HasMaxLength(80)
            .IsRequired();

        builder
            .HasOne(e => e.Cliente)
            .WithMany(c => c.Nacionalidades)
            .HasForeignKey(e => e.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}