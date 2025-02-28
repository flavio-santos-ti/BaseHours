using BaseHours.Application.Dtos;

namespace BaseHours.Application.Interfaces;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllAsync();
    Task<ClientDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ClientDto>> SearchByNameAsync(string name);
    Task AddAsync(ClientDto clientDto);
    Task UpdateAsync(ClientDto clientDto);
    Task DeleteAsync(Guid id);
}
