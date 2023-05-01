using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class RegistroDeEventoMap : IEntityTypeConfiguration<RegistroDeEvento>
{
    public void Configure(EntityTypeBuilder<RegistroDeEvento> builder)
    {
        builder.Property(c => c.DataHoraEvento)
            .IsRequired(false);
        
        builder.Property(c => c.ComputadorId)
            .IsRequired(false);
        
        builder.Property(c => c.Descricao)
            .IsRequired(false);
        
        builder.Property(c => c.ClienteId)
            .IsRequired(false);
        
        builder.Property(c => c.TipoOperacaoId)
            .IsRequired(false);
        
        builder.Property(c => c.UsuarioId)    
            .IsRequired(false);
        
        builder.Property(c => c.FuncaoMenuId)
            .IsRequired(false);

        builder
            .HasOne(c => c.Cliente)
            .WithMany(c => c.RegistroDeEventos)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.TipoOperacao)
            .WithMany(c => c.RegistroDeEventos)
            .HasForeignKey(c => c.TipoOperacaoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.Usuario)
            .WithMany(c => c.RegistroDeEventos)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.FuncaoMenu)
            .WithMany(c => c.RegistroDeEventos)
            .HasForeignKey(c => c.FuncaoMenuId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.Administrador)
            .WithMany(c => c.RegistroDeEventos)
            .HasForeignKey(c => c.AdministradorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}