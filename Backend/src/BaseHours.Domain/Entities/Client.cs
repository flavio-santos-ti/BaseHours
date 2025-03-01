using FDS.UuidV7.NetCore;

namespace BaseHours.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Client() // EF Core requires a parameterless constructor
    { 
        Name = string.Empty;
    } 

    public Client(Guid id, string name)
    {
        Id = UuidV7.Generate();
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Client name cannot be empty.");
        CreatedAt = DateTime.Now;
    }
}
