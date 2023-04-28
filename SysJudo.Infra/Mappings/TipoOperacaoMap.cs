using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class TipoOperacaoMap : IEntityTypeConfiguration<TipoOperacao>
{
    public void Configure(EntityTypeBuilder<TipoOperacao> builder)
    {
        builder.Property(c => c.Sigla)
            .IsRequired();

        builder.Property(c => c.Descricao)
            .IsRequired();

        builder.HasOne(c => c.Cliente)
            .WithMany(c => c.TiposOperacoes)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}