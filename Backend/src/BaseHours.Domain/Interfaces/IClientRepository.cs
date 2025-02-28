using BaseHours.Domain.Entities;

namespace BaseHours.Domain.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);
    Task<IEnumerable<Client>> GetAllAsync();
    Task<IEnumerable<Client>> SearchByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task DeleteAsync(Guid id);
}
