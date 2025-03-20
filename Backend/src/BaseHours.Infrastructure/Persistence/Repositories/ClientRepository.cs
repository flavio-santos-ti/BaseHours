using BaseHours.Domain.Entities;
using BaseHours.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<Client>> SearchByNameAsync(string name) =>
        await _context.Clients
            .Where(c => c.Name.Contains(name))
            .ToListAsync();

    public async Task<string> ExistsByNameAsync(string name)
    {
        bool exists = await _context.Clients.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        return exists ? "A client with this name already exists." : string.Empty;
    }

    public async Task<string> AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
        return "Client created successfully.";
    }

    public async Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await GetByIdAsync(id);
        if (client is not null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }
    }
}
