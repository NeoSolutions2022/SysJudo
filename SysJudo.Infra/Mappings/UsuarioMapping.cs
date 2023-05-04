using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder
            .Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(60);

        builder
            .Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(80);

        builder
            .Property(c => c.UltimoLogin)
            .IsRequired(false);

        builder
            .Property(c => c.DataExpiracao)
            .IsRequired(false);

        builder
            .Property(c => c.CriadoEm)
            .IsRequired();

        builder
            .Property(c => c.Senha)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .Property(c => c.Inadiplente)
            .HasDefaultValue(false)
            .IsRequired();

        builder
            .HasOne(u => u.Cliente)
            .WithMany(c => c.Usuarios)
            .HasForeignKey(u => u.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}