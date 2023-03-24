using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class AgremiacaoMapping : IEntityTypeConfiguration<Agremiacao>
{
    public void Configure(EntityTypeBuilder<Agremiacao> builder)
    {
        builder.Property(e => e.AlvaraLocacao)
            .HasDefaultValue(false);

        builder.Property(e => e.Anotacoes)
            .IsRequired(false)
            .HasMaxLength(1200);

        builder.Property(e => e.Bairro)
            .HasMaxLength(30);

        builder.Property(e => e.Cep)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(e => e.Cnpj)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(e => e.Complemento)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(e => e.Conteudo)
            .IsRequired(false);

        builder.Property(e => e.ContratoSocial)
            .HasDefaultValue(false);

        builder.Property(e => e.DataCnpj)
            .IsRequired(false);

        builder.Property(e => e.DataAta)
            .IsRequired(false);

        builder.Property(e => e.DocumentacaoAtualizada)
            .HasDefaultValue(false);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(e => e.Endereco)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(e => e.Estatuto)
            .HasDefaultValue(false);

        builder.Property(e => e.Fantasia)
            .IsRequired(false)
            .HasMaxLength(60);

        builder.Property(e => e.InscricaoEstadual)
            .IsRequired(false)
            .HasMaxLength(60);

        builder.Property(e => e.InscricaoMunicipal)
            .IsRequired(false)
            .HasMaxLength(60);

        builder.Property(e => e.Foto)
            .IsRequired(false);

        builder.Property(e => e.IdRegiao)
            .IsRequired();

        builder.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(e => e.Representante)
            .IsRequired()
            .HasMaxLength(60);

        builder
            .Property(e => e.DataFiliacao)
            .IsRequired();

        builder
            .Property(e => e.DataFiliacao)
            .IsRequired();

        builder.Property(e => e.Responsavel)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(e => e.Sigla)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.Telefone)
            .IsRequired()
            .HasMaxLength(60); 
        
        builder.Property(c => c.Cidade)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.Estado)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.Pais)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasOne(d => d.Regiao)
            .WithMany(p => p.Agremiacoes)
            .HasForeignKey(d => d.IdRegiao)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Cliente)
            .WithMany(p => p.Agremiacoes)
            .HasForeignKey(d => d.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}