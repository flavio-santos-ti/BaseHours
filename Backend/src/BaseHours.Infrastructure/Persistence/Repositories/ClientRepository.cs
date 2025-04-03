using BaseHours.Domain.Entities;
using BaseHours.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace BaseHours.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ApplicationDbContext _context;

    public ClientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByIdAsync(Guid id) =>
        await _context.Clients.FindAsync(id);

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _context.Clients.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Client>> SearchByNameAsync(string name)
    {
        return await _context.Clients
            .Where(c => c.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<string> ExistsByNameAsync(string name)
    {
        string normalizedInput = NormalizeName(name);

        bool exists = await _context.Clients
            .AnyAsync(c => NormalizeName(c.Name) == normalizedInput);

        return exists ? "A client with this name already exists." : string.Empty;
    }
    public async Task<string> AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
        return "Client created successfully.";
    }

    public async Task<string> UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
        return $"Client ID {client.Id} updated successfully.";
    }

    public async Task<string> DeleteAsync(Client client)
    {
        var id = client.Id;
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return $"Client ID {id} deleted successfully.";
    }

    private static string NormalizeName(string input)
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
