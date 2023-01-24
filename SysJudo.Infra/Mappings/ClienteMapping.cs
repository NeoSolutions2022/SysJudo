using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class ClienteMapping : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.Property(s => s.Sigla)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(s => s.Nome)
            .IsRequired()
            .HasMaxLength(60);
        
        builder.Property(s => s.PastaArquivo)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasOne(c => c.Sistema)
            .WithMany(s => s.Clientes)
            .HasForeignKey(c => c.IdSistema)
            .OnDelete(DeleteBehavior.Restrict);
    }
}