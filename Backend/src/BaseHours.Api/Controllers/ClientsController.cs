using BaseHours.Application.Dtos;
using BaseHours.Application.Interfaces;
using FDS.NetCore.ApiResponse.Models;
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

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ClientRequestDto request)
    {
        var response = await _clientService.AddAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _clientService.DeleteAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _clientService.GetAllAsync();
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var response = await _clientService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        var response = await _clientService.SearchByNameAsync(name);
        return StatusCode(response.StatusCode, response);
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
}
