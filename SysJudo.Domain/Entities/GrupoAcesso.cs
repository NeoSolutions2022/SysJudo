using SysJudo.Domain.Contracts;

namespace SysJudo.Domain.Entities;

public class GrupoAcesso : ITenant, IEntity, IAggregateRoot
{
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public bool Administrador { get; protected set; }
    public bool Desativado { get; set; }
    public int ClienteId { get; set; }
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual List<GrupoAcessoPermissao> Permissoes { get; set; } = new();

    // public override bool Validar(out ValidationResult validationResult)
    // {
    //     validationResult = new GrupoAcessoValidator().Validate(this);
    //     return validationResult.IsValid;
    // }
    public int Id { get; set; }
}