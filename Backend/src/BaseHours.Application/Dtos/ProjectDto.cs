namespace BaseHours.Application.Dtos;

/// <summary>
/// Project output DTO with optional client name
/// DTO de retorno do projeto com nome do cliente opcional
/// </summary>
public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid ClientId { get; set; }
    public string? ClientName { get; set; } // optional (eager loaded)
}