using FDS.UuidV7.NetCore;
using TextNormalizer;

namespace BaseHours.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = null!;
    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Project()
    {
        Name = string.Empty;
        NormalizedName = string.Empty;
    } 
    
    public Project(Guid clientId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty."); // Name required
        if (clientId == Guid.Empty)
            throw new ArgumentException("ClientId must be provided."); // Client required

        Id = UuidV7.Generate();
        ClientId = clientId;
        Name = name.Trim();
        NormalizedName = Normalizer.NormalizeText(Name); // Normalize name
        CreatedAt = DateTime.UtcNow;
    }

    public (bool IsValid, string? ErrorMessage) TrySetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (false, "Name cannot be empty.");
        if (name.Length < 3)
            return (false, "Name must have at least 3 characters.");

        Name = name.Trim();
        NormalizedName = Normalizer.NormalizeText(Name); // Normalize name
        return (true, null);
    }
}