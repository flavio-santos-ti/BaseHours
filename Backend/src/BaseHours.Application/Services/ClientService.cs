using BaseHours.Application.Dtos;
using BaseHours.Application.Interfaces;
using BaseHours.Domain.Entities;
using BaseHours.Domain.Interfaces;
using FDS.DbLogger.PostgreSQL.Published;
using FDS.NetCore.ApiResponse.Models;
using FDS.NetCore.ApiResponse.Results;
using FDS.NetCore.ApiResponse.Types;
using FDS.RequestTracking.Storage;
using Microsoft.AspNetCore.Http;

namespace BaseHours.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientService(IClientRepository clientRepository, IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
    {
        _clientRepository = clientRepository;
        _auditLogService = auditLogService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<ClientDto>> AddAsync(ClientRequestDto request)
    {
        try
        {
            string msg;

            var requestId = _httpContextAccessor.HttpContext?.TraceIdentifier;

            if (!string.IsNullOrEmpty(requestId))
            {
                // Retrieves the request data stored by the RequestDataFilter.
                var requestData = RequestDataStorage.GetData(requestId);

                if (requestData is not null)
                {
                    // Persisting the collected data in the audit database.
                    await _auditLogService.LogInfoAsync($"Request Data - {requestData.Method} {requestData.Path}{requestData.QueryParams} - {requestData.Timestamp}");

                    // Clears the data after persistence.
                    RequestDataStorage.ClearData(requestId);
                }
            }

            if (await _clientRepository.ExistsByNameAsync(request.Name))
            {
                msg = "A client with this name already exists.";

                await _auditLogService.LogValidationErrorAsync(msg, request);

                return Result.CreateValidationError<ClientDto>(msg);
            }

            var client = new Client(Guid.NewGuid(), request.Name);
            await _clientRepository.AddAsync(client);
            var clientDto = new ClientDto { Id = client.Id, Name = client.Name };

            msg = "Client created successfully.";

            await _auditLogService.LogCreateAsync(msg, request, clientDto);

            return Result.CreateSuccess("Client created successfully.", clientDto);
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
                return Result.CreateValidationError<ClientDto>(errorMessage ?? "Unknown validation error");
            }

            var updatedClientDto = new ClientDto { Id = existingClient.Id, Name = existingClient.Name };

            return Result.CreateSuccess("Client updated successfully.", updatedClientDto);
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
