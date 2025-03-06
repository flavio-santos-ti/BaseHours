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
                actionType: ActionType.VALIDATION_ERROR,
                message: "A client with this name already exists."
            );
        }

        var client = new Client(Guid.NewGuid(), request.Name);
        await _clientRepository.AddAsync(client);
        var clientDto = new ClientDto { Id = client.Id, Name = client.Name };

        return Result.Create(
            actionType: ActionType.CREATE, 
            message: "Client created successfully.", 
            data: clientDto
        );
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client is null)
        {
            return Result.Create<bool>(
                actionType: ActionType.NOT_FOUND,
                message: "Client not found."
            );
        }

        await _clientRepository.DeleteAsync(id);

        return Result.Create<bool>(
            actionType: ActionType.DELETE,
            message: "Client deleted successfully."
        );
    }

    public async Task<Response<IEnumerable<ClientDto>>> GetAllAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        var clientDtos = clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });

        return Result.Create(
            actionType: ActionType.READ,
            message: clients.Any() ? "Clients retrieved successfully." : "No clients found.",
            data: clientDtos
        );
    }

    public async Task<Response<ClientDto>> GetByIdAsync(string id)
    {
        if (!Guid.TryParse(id, out Guid validGuid))
        {
            return Result.Create<ClientDto>(
                actionType: ActionType.VALIDATION_ERROR,
                message: "Invalid client ID format."
            );
        }

        var client = await _clientRepository.GetByIdAsync(validGuid);

        if (client is null)
        {
            return Result.Create<ClientDto>(
                actionType: ActionType.NOT_FOUND,
                message: "Client not found."
            );
        }

        var clientDto = new ClientDto { Id = client.Id, Name = client.Name };

        return Result.Create(
            actionType: ActionType.READ,
            message: "Client retrieved successfully.",
            data: clientDto
        );
    }

    public async Task<Response<IEnumerable<ClientDto>>> SearchByNameAsync(string name)
    {
        var clients = await _clientRepository.SearchByNameAsync(name);
        var clientDtos = clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });

        return Result.Create(
            actionType: ActionType.READ,
            message: clients.Any() ? "Clients retrieved successfully." : "No clients found with the given name.",
            data: clientDtos
        );
    }
    public async Task UpdateAsync(ClientDto clientDto)
    {
        var client = new Client(clientDto.Id, clientDto.Name);
        await _clientRepository.UpdateAsync(client);
    }
}
