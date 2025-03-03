using BaseHours.Application.Dtos;
using BaseHours.Application.Interfaces;
using BaseHours.Domain.Entities;
using BaseHours.Domain.Interfaces;
using FDS.NetCore.ApiResponse.Models;
using FDS.NetCore.ApiResponse.Results;
using FDS.NetCore.ApiResponse.Types;

namespace BaseHours.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Response<ClientDto>> AddAsync(ClientRequestDto request)
    {
        if (await _clientRepository.ExistsByNameAsync(request.Name))
        {
            return Result.Create<ClientDto>(
                ActionType.VALIDATION_ERROR,
                "A client with this name already exists."
            );
        }

        var client = new Client(Guid.NewGuid(), request.Name);
        await _clientRepository.AddAsync(client);
        var clientDto = new ClientDto { Id = client.Id, Name = client.Name };

        return Result.Create(ActionType.CREATE, "Client created successfully.", clientDto);
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client is null)
        {
            return Result.Create<bool>(
                ActionType.NOT_FOUND,
                "Client not found."
            );
        }

        await _clientRepository.DeleteAsync(id);

        return Result.Create<bool>(
            ActionType.DELETE,
            "Client deleted successfully."
        );
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

    public async Task UpdateAsync(ClientDto clientDto)
    {
        var client = new Client(clientDto.Id, clientDto.Name);
        await _clientRepository.UpdateAsync(client);
    }
}
