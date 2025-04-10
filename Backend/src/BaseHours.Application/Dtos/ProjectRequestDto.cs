namespace BaseHours.Application.Dtos;


/// <summary>
/// Project request DTO used for creation or update
/// DTO de entrada do projeto usado para criação ou atualização
/// </summary>
public class ProjectRequestDto
{
    public string Name { get; set; } = null!;
    public Guid ClientId { get; set; }
}