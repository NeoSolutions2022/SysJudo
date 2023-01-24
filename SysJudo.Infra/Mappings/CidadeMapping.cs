using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class CidadeMapping : IEntityTypeConfiguration<Cidade>
{
    public void Configure(EntityTypeBuilder<Cidade> builder)
    {
        builder
            .Property(e => e.Descricao)
            .HasMaxLength(60)
            .IsUnicode(false);

        builder
            .Property(e => e.Sigla)
            .HasMaxLength(10)
            .IsUnicode(false);

        builder
            .HasOne(d => d.Pais)
            .WithMany(p => p.Cidades)
            .HasForeignKey(d => d.IdPais)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(d => d.Estado)
            .WithMany(p => p.Cidades)
            .HasForeignKey(d => d.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);


        builder
            .HasOne(c => c.Cliente)
            .WithMany(cl => cl.Cidades)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}