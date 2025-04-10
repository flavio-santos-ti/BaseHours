using BaseHours.Domain.Entities;
using BaseHours.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using TextNormalizer;

namespace BaseHours.Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id) =>
        await _context.Projects
            .Include(p => p.Client) // Include navigation
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.Client)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> SearchByNameAsync(string name)
    {
        return await _context.Projects
            .Where(p => p.Name.Contains(name))
            .Include(p => p.Client)
            .ToListAsync();
    }

    public async Task<string> ExistsByNameAsync(string name, Guid clientId)
    {
        string normalizedInput = Normalizer.NormalizeText(name);

        bool exists = await _context.Projects
            .AnyAsync(p => p.NormalizedName == normalizedInput && p.ClientId == clientId);

        return exists ? "A project with this name already exists for this client." : string.Empty;
    }

    public async Task<string> AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
        return "Project created successfully.";
    }

    public async Task<string> UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return $"Project ID {project.Id} updated successfully.";
    }

    public async Task<string> DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return $"Project ID {project.Id} deleted successfully.";
    }
}