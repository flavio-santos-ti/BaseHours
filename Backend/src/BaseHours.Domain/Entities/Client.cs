using FDS.UuidV7.NetCore;
using TextNormalizer;

namespace BaseHours.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Client() // EF Core requires a parameterless constructor
    {
        Name = string.Empty;
        NormalizedName = string.Empty;
    }

    public Client(string name)
    {
        Id = UuidV7.Generate();
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Client name cannot be empty.");
        NormalizedName = Normalizer.Normalize(name);
        CreatedAt = DateTime.Now;
    }

    public (bool IsValid, string? ErrorMessage) TrySetName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return (false, "Name cannot be empty.");

        if (newName.Length < 3)
            return (false, "Name must have at least 3 characters.");

        Name = newName;
        NormalizedName = Normalizer.Normalize(newName);
        return (true, null);
    }
}
