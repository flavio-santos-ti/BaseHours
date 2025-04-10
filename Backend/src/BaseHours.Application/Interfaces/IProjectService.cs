using BaseHours.Application.Dtos;
using FDS.NetCore.ApiResponse.Models;

namespace BaseHours.Application.Interfaces;

/// <summary>
/// Interface for project operations in the application layer
/// Interface para operações de projeto na camada de aplicação
/// </summary>
public interface IProjectService
{
    Task<Response<ProjectDto>> AddAsync(ProjectRequestDto request);
    Task<Response<ProjectDto>> UpdateAsync(ProjectDto request);
    Task<Response<bool>> DeleteAsync(Guid id);
    Task<Response<IEnumerable<ProjectDto>>> GetAllAsync();
    Task<Response<ProjectDto>> GetByIdAsync(string id);
    Task<Response<IEnumerable<ProjectDto>>> SearchByNameAsync(string name);
}