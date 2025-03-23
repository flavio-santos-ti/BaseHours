using BaseHours.Domain.Entities;

namespace BaseHours.Domain.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);
    Task<IEnumerable<Client>> GetAllAsync();
    Task<IEnumerable<Client>> SearchByNameAsync(string name);
    Task<string> ExistsByNameAsync(string name);
    Task<string> AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task<string> DeleteAsync(Client client);
}
