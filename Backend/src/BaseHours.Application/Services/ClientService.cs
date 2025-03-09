using BaseHours.Application.Dtos;
using BaseHours.Application.Interfaces;
using BaseHours.Domain.Entities;
using BaseHours.Domain.Interfaces;
using FDS.DbLogger.PostgreSQL.Published;
using FDS.NetCore.ApiResponse.Models;
using FDS.NetCore.ApiResponse.Results;
using FDS.NetCore.ApiResponse.Types;

namespace BaseHours.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IAuditLogService _auditLogService;

    public ClientService(IClientRepository clientRepository, IAuditLogService auditLogService)
    {
        _clientRepository = clientRepository;
        _auditLogService = auditLogService;
    }

    public async Task<Response<ClientDto>> AddAsync(ClientRequestDto request)
    {
        try
        {
            if (await _clientRepository.ExistsByNameAsync(request.Name))
            {
                string msg = "A client with this name already exists.";

                await _auditLogService.LogValidationErrorAsync(msg, request);

                return Result.Create<ClientDto>(
                    actionType: ActionType.VALIDATION_ERROR,
                    message: msg
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
        catch (Exception ex)
        {
            return Result.Create<ClientDto>(
                actionType: ActionType.ERROR,
                message: $"An unexpected error occurred: {ex.Message}"
            );
        }
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        try
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
        catch (Exception ex)
        {
            return Result.Create<bool>(
                actionType: ActionType.ERROR,
                message: $"An unexpected error occurred: {ex.Message}"
            );
        }
    }

    public async Task<Response<IEnumerable<ClientDto>>> GetAllAsync()
    {
        try
        {
            var clients = await _clientRepository.GetAllAsync();
            var clientDtos = clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });

            return Result.Create(
                actionType: ActionType.READ,
                message: clients.Any() ? "Clients retrieved successfully." : "No clients found.",
                data: clientDtos
            );
        }
        catch (Exception ex)
        {
            return Result.Create<IEnumerable<ClientDto>>(
                actionType: ActionType.ERROR,
                message: $"An unexpected error occurred: {ex.Message}"
            );
        }
    }

    public async Task<Response<ClientDto>> GetByIdAsync(string id)
    {
        try
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
        catch (Exception ex)
        {
            return Result.Create<ClientDto>(
                actionType: ActionType.ERROR,
                message: $"An unexpected error occurred: {ex.Message}"
            );
        }
    }

    public async Task<Response<IEnumerable<ClientDto>>> SearchByNameAsync(string name)
    {
        try
        {
            var clients = await _clientRepository.SearchByNameAsync(name);
            var clientDtos = clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });

            return Result.Create(
                actionType: ActionType.READ,
                message: clients.Any() ? "Clients retrieved successfully." : "No clients found with the given name.",
                data: clientDtos
            );
        }
        catch (Exception ex)
        {
            return Result.Create<IEnumerable<ClientDto>>(
                actionType: ActionType.ERROR,
                message: $"An unexpected error occurred: {ex.Message}"
            );
        }  
    }

    public async Task<Response<ClientDto>> UpdateAsync(ClientDto clientDto)
    {
        try
        {
            var existingClient = await _clientRepository.GetByIdAsync(clientDto.Id);
            if (existingClient is null)
            {
                return Result.Create<ClientDto>(
                    actionType: ActionType.NOT_FOUND,
                    message: "Client not found."
                );
            }

            var (isValid, errorMessage) = existingClient.UpdateName(clientDto.Name);
            if (!isValid)
            {
                return Result.Create<ClientDto>(
                    actionType: ActionType.VALIDATION_ERROR,
                    message: errorMessage!
                );
            }

            var updatedClientDto = new ClientDto { Id = existingClient.Id, Name = existingClient.Name };

            return Result.Create(
                actionType: ActionType.UPDATE,
                message: "Client updated successfully.",
                data: updatedClientDto
            );
        }
        catch (Exception ex)
        {
            return Result.Create<ClientDto>(
                actionType: ActionType.ERROR,
                message: $"An unexpected error occurred: {ex.Message}"
            );
        }
    }
}
