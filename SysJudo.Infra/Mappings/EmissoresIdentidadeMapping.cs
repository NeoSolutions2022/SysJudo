using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class EmissoresIdentidadeMapping : IEntityTypeConfiguration<EmissoresIdentidade>
{
    public void Configure(EntityTypeBuilder<EmissoresIdentidade> builder)
    {
        builder.Property(c => c.Sigla)
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(c => c.Descricao)
            .HasMaxLength(80)
            .IsRequired();

        builder
            .HasOne(e => e.Cliente)
            .WithMany(c => c.EmissoresIdentidades)
            .HasForeignKey(e => e.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}