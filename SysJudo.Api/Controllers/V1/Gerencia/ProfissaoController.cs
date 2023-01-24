using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.EmissoresIdentidade;
using SysJudo.Application.Dto.Profissao;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Gerencia;

[Route("v{version:apiVersion}/Gerencia/[controller]")]
public class ProfissaoController : MainController
{
    private readonly IProfissaoService _profissaoService;
    public ProfissaoController(INotificator notificator, IProfissaoService profissaoService) : base(notificator)
    {
        _profissaoService = profissaoService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listar Profissões.", Tags = new[] { "Gerencia - Profissões" })]
    [ProducesResponseType(typeof(PagedDto<ProfissaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarProfissaoDto dto)
    {
        var profissao = await _profissaoService.Buscar(dto);
        return OkResponse(profissao);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Profissões.", Tags = new[] { "Gerencia - Profissões" })]
    [ProducesResponseType(typeof(ProfissaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var profissao = await _profissaoService.ObterPorId(id);
        return OkResponse(profissao);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Profissões.", Tags = new[] { "Gerencia - Profissões" })]
    [ProducesResponseType(typeof(ProfissaoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateProfissaoDto dto)
    {
        var profissao = await _profissaoService.Adicionar(dto);
        return CreatedResponse("", profissao);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Profissões.", Tags = new[] { "Gerencia - Profissões" })]
    [ProducesResponseType(typeof(ProfissaoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateProfissaoDto dto)
    {
        var profissao = await _profissaoService.Alterar(id, dto);
        return OkResponse(profissao);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Profissões.", Tags = new[] { "Gerencia - Profissões" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _profissaoService.Remover(id);
        return NoContentResponse();
    }
}