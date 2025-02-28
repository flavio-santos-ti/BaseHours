using BaseHours.Application.Dtos;
using BaseHours.Application.Interfaces;
using BaseHours.Domain.Entities;
using BaseHours.Domain.Interfaces;

namespace BaseHours.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<IEnumerable<ClientDto>> GetAllAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        return clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });
    }

    public async Task<ClientDto?> GetByIdAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        return client is not null ? new ClientDto { Id = client.Id, Name = client.Name } : null;
    }

    public async Task<IEnumerable<ClientDto>> SearchByNameAsync(string name)
    {
        var clients = await _clientRepository.SearchByNameAsync(name);
        return clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });
    }

    public async Task AddAsync(ClientDto clientDto)
    {
        if (await _clientRepository.ExistsByNameAsync(clientDto.Name))
        {
            throw new InvalidOperationException("A client with this name already exists.");
        }

        var client = new Client(Guid.NewGuid(), clientDto.Name);
        await _clientRepository.AddAsync(client);
    }

    public async Task UpdateAsync(ClientDto clientDto)
    {
        var client = new Client(clientDto.Id, clientDto.Name);
        await _clientRepository.UpdateAsync(client);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _clientRepository.DeleteAsync(id);
    }
}
