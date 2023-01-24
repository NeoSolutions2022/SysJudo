using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class AtletaMapping : IEntityTypeConfiguration<Atleta>
{
    public void Configure(EntityTypeBuilder<Atleta> builder)
    {
        builder.Property(c => c.RegistroFederacao)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(c => c.RegistroConfederacao)
            .IsRequired(false)
            .HasMaxLength(10);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(c => c.DataNascimento)
            .IsRequired();

        builder.Property(c => c.DataFiliacao)
            .IsRequired();

        builder.Property(c => c.IdAgremiacao)
            .IsRequired();

        builder.Property(c => c.Cep)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(c => c.Endereco)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(c => c.Bairro)
            .IsRequired(false)
            .HasMaxLength(30);

        builder.Property(c => c.Complemento)
            .IsRequired(false)
            .HasMaxLength(60);

        builder.Property(c => c.IdCidade)
            .IsRequired();

        builder.Property(c => c.IdEstado)
            .IsRequired();

        builder.Property(c => c.IdPais)
            .IsRequired();

        builder.Property(c => c.Telefone)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(c => c.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(c => c.Identidade)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.DataIdentidade)
            .IsRequired();

        builder.Property(c => c.IdEmissor)
            .IsRequired();

        builder.Property(c => c.IdNacionalidade)
            .IsRequired();

        builder.Property(c => c.IdProfissaoAtleta)
            .IsRequired();

        builder.Property(c => c.NomePai)
            .IsRequired(false)
            .HasMaxLength(60);

        builder.Property(c => c.NomeMae)
            .IsRequired(false)
            .HasMaxLength(60);

        builder.Property(c => c.IdFaixa)
            .IsRequired();

        builder.Property(c => c.IdSexo)
            .IsRequired();

        builder.Property(c => c.IdEstadoCivil)
            .IsRequired();

        builder.Property(e => e.Foto)
            .IsRequired(false)
            .HasMaxLength(255)
            .IsFixedLength();

        builder
            .HasOne(a => a.Faixa)
            .WithMany(f => f.Atletas)
            .HasForeignKey(a => a.IdFaixa);

        builder
            .HasOne(a => a.Sexo)
            .WithMany(s => s.Atletas)
            .HasForeignKey(a => a.IdSexo);

        builder
            .HasOne(a => a.EstadoCivil)
            .WithMany(e => e.Atletas)
            .HasForeignKey(a => a.IdEstadoCivil);

        builder
            .HasOne(a => a.Profissao)
            .WithMany(e => e.Atletas)
            .HasForeignKey(a => a.IdProfissaoAtleta);

        builder
            .HasOne(a => a.EmissoresIdentidade)
            .WithMany(e => e.Atletas)
            .HasForeignKey(a => a.IdEmissor);

        builder
            .HasOne(a => a.Nacionalidade)
            .WithMany(e => e.Atletas)
            .HasForeignKey(a => a.IdNacionalidade);

        builder
            .HasOne(a => a.Cidade)
            .WithMany(e => e.Atletas)
            .HasForeignKey(a => a.IdCidade);

        builder
            .HasOne(a => a.Estado)
            .WithMany(e => e.Atletas)
            .HasForeignKey(a => a.IdEstado);

        builder
            .HasOne(a => a.Pais)
            .WithMany(e => e.Atletas)
            .HasForeignKey(a => a.IdPais);

        builder
            .HasOne(a => a.Agremiacao)
            .WithMany(e => e.Atletas)
            .HasForeignKey(a => a.IdAgremiacao);

        builder
            .HasOne(a => a.Cliente)
            .WithMany(e => e.Atletas)
            .HasForeignKey(a => a.ClienteId);
    }
}