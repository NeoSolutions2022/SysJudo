using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class EstadoCivilMapping : IEntityTypeConfiguration<EstadoCivil>
{
    public void Configure(EntityTypeBuilder<EstadoCivil> builder)
    {
        builder
            .Property(e => e.Sigla)
            .IsRequired();
        
        builder
            .Property(s => s.Descricao)
            .HasMaxLength(60)
            .IsRequired();

        builder
            .HasOne(e => e.Cliente)
            .WithMany(c => c.EstadosCivis)
            .HasForeignKey(s => s.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}