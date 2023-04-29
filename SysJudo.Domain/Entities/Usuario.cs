using FluentValidation.Results;
using SysJudo.Domain.Contracts;
using SysJudo.Domain.Validation;

namespace SysJudo.Domain.Entities;

public class Usuario : Entity, IAggregateRoot
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CriadoEm { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public string Senha { get; set; } = null!;
    public bool Inadiplente { get; set; }
    
    public int ClienteId { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;
    public virtual List<RegistroDeEvento> RegistroDeEventos { get; set; } = new();
    
    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new UsuarioValidator().Validate(this);
        return validationResult.IsValid;
    }
}