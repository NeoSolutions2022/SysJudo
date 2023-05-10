using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Infra.Mappings.MappingsFiltro;

public class GrupoDeAcessoFiltroMapping : IEntityTypeConfiguration<GrupoDeAcessoFiltro>
{
    public void Configure(EntityTypeBuilder<GrupoDeAcessoFiltro> builder)
    {
        builder.HasKey(c => c.Identificador);
        
        builder.Property(c => c.Descricao)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Administrador)
            .HasDefaultValue(false);
        
        builder.Property(c => c.Desativado)
            .HasDefaultValue(false);
    }
}