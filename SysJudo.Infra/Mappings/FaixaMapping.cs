using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class FaixaMapping : IEntityTypeConfiguration<Faixa>
{
    public void Configure(EntityTypeBuilder<Faixa> builder)
    {
        builder
            .Property(s => s.Sigla)
            .IsRequired()
            .HasMaxLength(10);
                
        builder.Property(s => s.Descricao)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(s => s.MesesCarencia)
            .IsRequired();
                
        builder.Property(s => s.IdadeMinima)
            .IsRequired();
                
        builder.Property(s => s.OrdemExibicao)
            .IsRequired();

        builder
            .HasOne(c => c.Cliente)
            .WithMany(s => s.Faixas)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}