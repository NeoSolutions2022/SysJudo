using System.ComponentModel;

namespace SysJudo.Core.Enums;

public enum ETipoUsuario
{
    [Description("Administrador")]
    Administrador = 1,
    [Description("Comum")]
    Comum = 2
}