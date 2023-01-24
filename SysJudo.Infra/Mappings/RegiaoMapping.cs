using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class RegiaoMapping : IEntityTypeConfiguration<Regiao>
{
    public void Configure(EntityTypeBuilder<Regiao> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Anotacoes)
            .HasColumnType("text")
            .HasColumnName("ANOTACOES");

        builder.Property(e => e.Bairro)
            .HasMaxLength(30);

        builder.Property(e => e.Cep)
            .HasMaxLength(8);

        builder.Property(e => e.Complemento)
            .HasMaxLength(60);

        builder.Property(e => e.Descricao)
            .HasMaxLength(60);

        builder.Property(e => e.Email)
            .HasMaxLength(60);

        builder.Property(e => e.Endereco)
            .HasMaxLength(60);

        builder.Property(e => e.Responsavel)
            .HasMaxLength(60);

        builder.Property(e => e.Sigla)
            .HasMaxLength(10);

        builder.Property(e => e.Telefone)
            .HasMaxLength(60);

        builder.HasOne(d => d.Pais)
            .WithMany(p => p.Regioes)
            .HasForeignKey(d => d.IdPais)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Estado)
            .WithMany(p => p.Regioes)
            .HasForeignKey(d => d.IdEstado)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Cidade)
            .WithMany(p => p.Regioes)
            .HasForeignKey(d => d.IdCidade)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(d => d.Cliente)
            .WithMany(p => p.Regioes)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}