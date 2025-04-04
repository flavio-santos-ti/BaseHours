using FDS.UuidV7.NetCore;
using System.Globalization;
using System.Text;

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
        NormalizedName = NormalizeName(name);
        CreatedAt = DateTime.Now;
    }

    public (bool IsValid, string? ErrorMessage) TrySetName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return (false, "Name cannot be empty.");

        if (newName.Length < 3)
            return (false, "Name must have at least 3 characters.");

        Name = newName;
        NormalizedName = NormalizeName(newName);
        return (true, null);
    }

    public static string NormalizeName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string normalizedFormD = input.Normalize(NormalizationForm.FormD);
        var withoutDiacritics = new StringBuilder();

        foreach (char c in normalizedFormD)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                withoutDiacritics.Append(c);
        }

        return withoutDiacritics
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Trim()
            .ToUpperInvariant();
    }
}
