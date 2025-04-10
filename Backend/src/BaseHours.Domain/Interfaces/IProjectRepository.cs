using BaseHours.Domain.Entities;

namespace BaseHours.Domain.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<IEnumerable<Project>> GetAllAsync();
    Task<IEnumerable<Project>> SearchByNameAsync(string name);
    Task<string> ExistsByNameAsync(string name, Guid clientId);
    Task<string> AddAsync(Project project);
    Task<string> UpdateAsync(Project project);
    Task<string> DeleteAsync(Project project);
}
