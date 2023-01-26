using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Cliente;
using SysJudo.Application.Notifications;

namespace SysJudo.Api.Controllers.V1.Administrador;

[Route("v{version:apiVersion}/Administrador/[controller]")]
public class ClienteController : MainController
{
    private readonly IClienteService _clienteService;
    public ClienteController(INotificator notificator, IClienteService clienteService) : base(notificator)
    {
        _clienteService = clienteService;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Listaar Cliente.", Tags = new [] { "Administrador - Cliente" })]
    [ProducesResponseType(typeof(PagedDto<ClienteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Buscar([FromQuery] BuscarClienteDto dto)
    {
        var cliente = await _clienteService.Buscar(dto);
        return OkResponse(cliente);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obter Cliente.", Tags = new [] { "Administrador - Cliente" })]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var cliente = await _clienteService.ObterPorId(id);
        return OkResponse(cliente);
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar Cliente.", Tags = new [] { "Administrador - Cliente" })]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateClienteDto dto)
    {
        var cliente = await _clienteService.Adicionar(dto);
        return CreatedResponse("", cliente);
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar Cliente.", Tags = new [] { "Administrador - Cliente" })]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Alterar(int id, [FromBody] UpdateClienteDto dto)
    {
        var cliente = await _clienteService.Alterar(id, dto);
        return OkResponse(cliente);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover Cliente.", Tags = new[] { "Administrador - Cliente" })]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(int id)
    {
        await _clienteService.Remover(id);
        return NoContentResponse();
    }
}