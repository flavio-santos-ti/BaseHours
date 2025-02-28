using BaseHours.Application.Dtos;
using BaseHours.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaseHours.Api.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _clientService.GetAllAsync();
        return Ok(clients);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var client = await _clientService.GetByIdAsync(id);
        return client is not null ? Ok(client) : NotFound(new { message = "Client not found" });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        var clients = await _clientService.SearchByNameAsync(name);
        return Ok(clients);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ClientDto clientDto)
    {
        try
        {
            await _clientService.AddAsync(clientDto);
            return CreatedAtAction(nameof(GetById), new { id = clientDto.Id }, clientDto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // Retorna erro 409 se o nome já existir
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ClientDto clientDto)
    {
        if (id != clientDto.Id)
        {
            return BadRequest(new { message = "ID mismatch between route and body" });
        }

        await _clientService.UpdateAsync(clientDto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _clientService.DeleteAsync(id);
        return NoContent();
    }
}
