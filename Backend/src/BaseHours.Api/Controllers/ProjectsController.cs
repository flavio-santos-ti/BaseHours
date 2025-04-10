using BaseHours.Application.Dtos;
using BaseHours.Application.Interfaces;
using FDS.RequestTracking.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BaseHours.Api.Controllers;

[ApiController]
[Route("api/projects")]
[ServiceFilter(typeof(RequestDataFilter))] // Intercepts to enrich request context
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Adds a new project
    /// Adiciona um novo projeto
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] ProjectRequestDto request)
    {
        var response = await _projectService.AddAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Deletes a project by ID
    /// Deleta um projeto pelo ID
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _projectService.DeleteAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Gets all projects
    /// Obtém todos os projetos
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _projectService.GetAllAsync();
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Gets a project by ID
    /// Obtém um projeto pelo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var response = await _projectService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Searches projects by name
    /// Pesquisa projetos pelo nome
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        var response = await _projectService.SearchByNameAsync(name);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Updates a project
    /// Atualiza um projeto
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] ProjectDto request)
    {
        var response = await _projectService.UpdateAsync(request);
        return StatusCode(response.StatusCode, response);
    }
}
