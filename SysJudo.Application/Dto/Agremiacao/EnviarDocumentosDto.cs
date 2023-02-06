using Microsoft.AspNetCore.Http;

namespace SysJudo.Application.Dto.Agremiacao;

public class EnviarDocumentosDto
{
    public List<IFormFile> Documentos { get; set; }
}