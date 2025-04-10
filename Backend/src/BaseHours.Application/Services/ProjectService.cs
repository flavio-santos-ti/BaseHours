using BaseHours.Application.Dtos;
using BaseHours.Application.Interfaces;
using BaseHours.Domain.Entities;
using BaseHours.Domain.Interfaces;
using FDS.DbLogger.PostgreSQL.Published;
using FDS.NetCore.ApiResponse.Models;
using FDS.NetCore.ApiResponse.Results;
using FDS.RequestTracking.Storage;
using Microsoft.AspNetCore.Http;

namespace BaseHours.Application.Services;

/// <summary>
/// Application service for managing projects
/// Serviço de aplicação para gerenciamento de projetos
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjectService(
        IProjectRepository projectRepository,
        IClientRepository clientRepository,
        IAuditLogService auditLogService,
        IHttpContextAccessor httpContextAccessor)
    {
        _projectRepository = projectRepository;
        _clientRepository = clientRepository;
        _auditLogService = auditLogService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<ProjectDto>> AddAsync(ProjectRequestDto request)
    {
        string requestId = string.Empty;
        string msg = string.Empty;

        try
        {
            requestId = await _auditLogService.LogStartAsync("Project creation process started.");

            msg = await _projectRepository.ExistsByNameAsync(request.Name, request.ClientId);
            if (!string.IsNullOrEmpty(msg))
            {
                await _auditLogService.LogValidationErrorAsync(msg, request);
                return Result.CreateValidationError<ProjectDto>(msg);
            }

            var client = await _clientRepository.GetByIdAsync(request.ClientId);
            if (client is null)
            {
                msg = "Associated client not found.";
                await _auditLogService.LogValidationErrorAsync(msg, request);
                return Result.CreateValidationError<ProjectDto>(msg);
            }

            var project = new Project(request.ClientId, request.Name.Trim());
            msg = await _projectRepository.AddAsync(project);

            var dto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                ClientId = project.ClientId,
                ClientName = client.Name
            };

            await _auditLogService.LogCreateAsync(msg, request, dto);
            return Result.CreateAdd(msg, dto);
        }
        catch (Exception ex)
        {
            msg = $"An unexpected error occurred: {ex.Message}";
            await _auditLogService.LogErrorAsync(msg);
            return Result.CreateError<ProjectDto>(msg);
        }
        finally
        {
            msg = "Project creation process completed.";
            await _auditLogService.LogEndAsync(msg);
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<ProjectDto>> UpdateAsync(ProjectDto request)
    {
        string requestId = string.Empty;
        string msg = string.Empty;

        try
        {
            requestId = await _auditLogService.LogStartAsync($"[START] - Updating project ID: {request.Id}");

            var project = await _projectRepository.GetByIdAsync(request.Id);
            if (project is null)
            {
                msg = $"Project ID {request.Id} not found.";
                await _auditLogService.LogNotFoundAsync(msg, request);
                return Result.CreateNotFound<ProjectDto>(msg);
            }

            var (isValid, error) = project.TrySetName(request.Name);
            if (!isValid)
            {
                msg = error ?? "Invalid name provided.";
                await _auditLogService.LogValidationErrorAsync(msg, request);
                return Result.CreateValidationError<ProjectDto>(msg);
            }

            msg = await _projectRepository.UpdateAsync(project);

            var dto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                ClientId = project.ClientId
            };

            await _auditLogService.LogUpdateAsync(msg, request, dto);
            return Result.CreateModify(dto);
        }
        catch (Exception ex)
        {
            msg = $"Unexpected error while updating project {request.Id}: {ex.Message}";
            await _auditLogService.LogErrorAsync(msg, request);
            return Result.CreateError<ProjectDto>(msg);
        }
        finally
        {
            msg = "Project update completed.";
            await _auditLogService.LogEndAsync(msg);
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        string requestId = string.Empty;
        string msg = string.Empty;

        try
        {
            requestId = await _auditLogService.LogStartAsync($"Deleting project ID: {id}");

            var project = await _projectRepository.GetByIdAsync(id);
            if (project is null)
            {
                msg = $"Project ID {id} not found.";
                await _auditLogService.LogNotFoundAsync(msg);
                return Result.CreateNotFound<bool>(msg);
            }

            msg = await _projectRepository.DeleteAsync(project);
            await _auditLogService.LogDeleteAsync(msg);

            return Result.CreateRemove<bool>(msg);
        }
        catch (Exception ex)
        {
            msg = $"Unexpected error while deleting project {id}: {ex.Message}";
            await _auditLogService.LogErrorAsync(msg);
            return Result.CreateError<bool>(msg);
        }
        finally
        {
            msg = "Project deletion completed.";
            await _auditLogService.LogEndAsync(msg);
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<IEnumerable<ProjectDto>>> GetAllAsync()
    {
        string requestId = string.Empty;
        string msg = string.Empty;

        try
        {
            msg = "Retrieving all projects.";
            requestId = await _auditLogService.LogStartAsync(msg);

            var projects = await _projectRepository.GetAllAsync();
            var dtos = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                ClientId = p.ClientId,
                ClientName = p.Client?.Name
            });

            msg = projects.Any()
                ? $"Projects retrieved successfully. Total: {projects.Count()}."
                : "No projects found.";

            await _auditLogService.LogReadAsync(msg);
            return Result.CreateGet<IEnumerable<ProjectDto>>(msg, dtos);
        }
        catch (Exception ex)
        {
            msg = $"Error retrieving projects: {ex.Message}";
            await _auditLogService.LogErrorAsync(msg);
            return Result.CreateError<IEnumerable<ProjectDto>>(msg);
        }
        finally
        {
            msg = "Project retrieval process completed.";
            await _auditLogService.LogEndAsync(msg);
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<ProjectDto>> GetByIdAsync(string id)
    {
        string requestId = string.Empty;
        string msg = string.Empty;

        try
        {
            msg = $"Retrieving project ID: {id}";
            requestId = await _auditLogService.LogStartAsync(msg);

            if (!Guid.TryParse(id, out Guid validId))
            {
                msg = "Invalid project ID format.";
                await _auditLogService.LogValidationErrorAsync(msg, id);
                return Result.CreateValidationError<ProjectDto>(msg);
            }

            var project = await _projectRepository.GetByIdAsync(validId);
            if (project is null)
            {
                msg = $"Project with ID {id} not found.";
                await _auditLogService.LogNotFoundAsync(msg);
                return Result.CreateNotFound<ProjectDto>(msg);
            }

            var dto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                ClientId = project.ClientId,
                ClientName = project.Client?.Name
            };

            msg = $"Project ID {id} retrieved successfully.";
            await _auditLogService.LogReadAsync(msg, dto);

            return Result.CreateGet(msg, dto);
        }
        catch (Exception ex)
        {
            msg = $"Unexpected error retrieving project {id}: {ex.Message}";
            await _auditLogService.LogErrorAsync(msg, id);
            return Result.CreateError<ProjectDto>(msg);
        }
        finally
        {
            msg = "Project retrieval by ID completed.";
            await _auditLogService.LogEndAsync(msg);
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<IEnumerable<ProjectDto>>> SearchByNameAsync(string name)
    {
        string requestId = string.Empty;
        string msg = string.Empty;

        try
        {
            msg = $"Searching projects by name: '{name}'";
            requestId = await _auditLogService.LogStartAsync(msg);

            var projects = await _projectRepository.SearchByNameAsync(name);
            var dtos = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                ClientId = p.ClientId,
                ClientName = p.Client?.Name
            });

            msg = projects.Any()
                ? $"Projects found with the name '{name}'. Total: {projects.Count()}."
                : $"No projects found with the name '{name}'.";

            await _auditLogService.LogReadAsync(msg, dtos);
            return Result.CreateGet(msg, dtos);
        }
        catch (Exception ex)
        {
            msg = $"Error searching projects: {ex.Message}";
            await _auditLogService.LogErrorAsync(msg);
            return Result.CreateError<IEnumerable<ProjectDto>>(msg);
        }
        finally
        {
            msg = "Project search process completed.";
            await _auditLogService.LogEndAsync(msg);
            RequestDataStorage.ClearData(requestId);
        }
    }
}