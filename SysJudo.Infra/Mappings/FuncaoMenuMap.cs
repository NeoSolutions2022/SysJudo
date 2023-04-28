using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysJudo.Domain.Entities;

namespace SysJudo.Infra.Mappings;

public class FuncaoMenuMap : IEntityTypeConfiguration<FuncaoMenu>
{
    public void Configure(EntityTypeBuilder<FuncaoMenu> builder)
    {
        builder.Property(c => c.Sigla)
            .IsRequired();
        
        builder.Property(c => c.Descricao)
            .IsRequired();

        builder.HasOne(c => c.Cliente)
            .WithMany(c => c.FuncoesMenus)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}