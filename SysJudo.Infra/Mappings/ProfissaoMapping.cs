using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class ProfissaoMapping : IEntityTypeConfiguration<Profissao>
{
    public void Configure(EntityTypeBuilder<Profissao> builder)
    {
        builder.Property(c => c.Sigla)
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(c => c.Descricao)
            .HasMaxLength(60)
            .IsRequired();

        builder
            .HasOne(e => e.Cliente)
            .WithMany(c => c.Profissoes)
            .HasForeignKey(e => e.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}