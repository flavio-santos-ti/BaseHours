using BaseHours.Application.Dtos;
using BaseHours.Application.Interfaces;
using BaseHours.Domain.Entities;
using BaseHours.Domain.Interfaces;
using FDS.DbLogger.PostgreSQL.Published;
using FDS.NetCore.ApiResponse.Models;
using FDS.NetCore.ApiResponse.Results;
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
        string requestId = string.Empty;
        try
        {
            requestId = await _auditLogService.LogInfoAsync("[START] - Client creation process started.", request);

            string msg = await _clientRepository.ExistsByNameAsync(request.Name);

            if (!string.IsNullOrEmpty(msg))
            {
                await _auditLogService.LogValidationErrorAsync(msg, request);
                return Result.CreateValidationError<ClientDto>(msg);
            }

            var client = new Client(request.Name);
            msg = await _clientRepository.AddAsync(client);
            var clientDto = new ClientDto { Id = client.Id, Name = client.Name };

            await _auditLogService.LogCreateAsync(msg, request, clientDto);

            return Result.CreateSuccess(msg, clientDto);
        }
        catch (Exception ex)
        {
            return Result.CreateError<ClientDto>($"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        string requestId = string.Empty;
        try
        {
            requestId = await _auditLogService.LogInfoAsync($"[START] - Client deletion process started for ID: {id}");
            string msg = string.Empty;

            var client = await _clientRepository.GetByIdAsync(id);

            if (client is null)
            {
                msg = $"Client ID {id} not found.";
                await _auditLogService.LogNotFoundAsync(msg);
                return Result.CreateNotFound<bool>(msg);
            }

            await _clientRepository.DeleteAsync(id);
            msg = $"Client ID {id} deleted successfully.";
            await _auditLogService.LogDeleteAsync(msg);

            return Result.CreateDelete<bool>(msg);
        }
        catch (Exception ex)
        {
            return Result.CreateError<bool>($"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<IEnumerable<ClientDto>>> GetAllAsync()
    {
        string requestId = string.Empty;
        try
        {
            requestId = await _auditLogService.LogInfoAsync("[START] - Retrieving all clients.");

            string msg = string.Empty;

            var clients = await _clientRepository.GetAllAsync();
            var clientDtos = clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });
            msg = clients.Any() ? $"Clients retrieved successfully. Total: {clients.Count()}." : "No clients found.";
            await _auditLogService.LogReadAsync(msg);

            return Result.CreateRead<IEnumerable<ClientDto>>(msg, clientDtos);
        }
        catch (Exception ex)
        {
            return Result.CreateError<IEnumerable<ClientDto>>($"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<ClientDto>> GetByIdAsync(string id)
    {
        string requestId = string.Empty;
        string msg;
        try
        {
            msg = $"Retrieving client with ID: {id}";
            requestId = await _auditLogService.LogStartAsync(msg);

            if (!Guid.TryParse(id, out Guid validGuid))
            {
                msg = "Invalid client ID format.";
                await _auditLogService.LogValidationErrorAsync(msg, id);

                return Result.CreateValidationError<ClientDto>(msg);
            }

            var client = await _clientRepository.GetByIdAsync(validGuid);

            if (client is null)
            {
                msg = $"Client with ID {id} not found.";
                await _auditLogService.LogNotFoundAsync(msg, id);
                return Result.CreateNotFound<ClientDto>(msg);
            }

            var clientDto = new ClientDto { Id = client.Id, Name = client.Name };
            msg = $"Client with ID {id} retrieved successfully.";
            await _auditLogService.LogReadAsync(msg, clientDto);

            return Result.CreateRead<ClientDto>(msg, clientDto);
        }
        catch (Exception ex)
        {
            msg = $"An unexpected error occurred while retrieving client with ID {id}: {ex.Message}";
            await _auditLogService.LogErrorAsync(msg, id);
            return Result.CreateError<ClientDto>(msg);
        }
        finally
        {
            await _auditLogService.LogEndAsync("Client retrieval by ID process completed.");
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<IEnumerable<ClientDto>>> SearchByNameAsync(string name)
    {
        string requestId = string.Empty;
        string msg = string.Empty;
        try
        {
            msg = $"Searching clients by name: '{name}'";
            requestId = await _auditLogService.LogStartAsync(msg);

            var clients = await _clientRepository.SearchByNameAsync(name);
            var clientDtos = clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });
            
            msg = clients.Any() ? $"Clients found with the name '{name}'. Total: {clients.Count()}." : $"No clients found with the name '{name}'.";

            await _auditLogService.LogReadAsync(msg, clientDtos);

            return Result.CreateRead<IEnumerable<ClientDto>>(msg, clientDtos);
        }
        catch (Exception ex)
        {
            msg = $"An unexpected error occurred while searching clients by name '{name}': {ex.Message}";
            await _auditLogService.LogErrorAsync(msg);
            return Result.CreateError<IEnumerable<ClientDto>>(msg);
        }
        finally
        {
            await _auditLogService.LogEndAsync("Client searc by name process completed.");
            RequestDataStorage.ClearData(requestId);
        }
    }

    public async Task<Response<ClientDto>> UpdateAsync(ClientDto request)
    {
        string requestId = string.Empty;
        string msg = string.Empty;

        try
        {
            msg = $"[START] - Client update process started for ID: {request.Id}";
            requestId = await _auditLogService.LogInfoAsync(msg, request);

            var existingClient = await _clientRepository.GetByIdAsync(request.Id);
            if (existingClient is null)
            {
                msg = $"Client ID {request.Id} not found.";
                await _auditLogService.LogNotFoundAsync(msg, request);
                return Result.CreateNotFound<ClientDto>("Client not found.");
            }

            var (isValid, errorMessage) = existingClient.UpdateName(request.Name);
            if (!isValid)
            {
                msg = errorMessage ?? "Unknown validation error";
                await _auditLogService.LogValidationErrorAsync(msg, request);
                return Result.CreateValidationError<ClientDto>(msg);
            }

            // Agora que a validação passou, criamos o DTO atualizado corretamente
            msg = $"Client ID {request.Id} updated successfully.";
            var updatedClientDto = new ClientDto { Id = existingClient.Id, Name = existingClient.Name };

            await _auditLogService.LogUpdateAsync(msg, request, updatedClientDto);

            return Result.CreateSuccess("Client updated successfully.", updatedClientDto);
        }
        catch (Exception ex)
        {
            msg = $"An unexpected error occurred while updating client ID {request.Id}: {ex.Message}";
            await _auditLogService.LogErrorAsync(msg, request);
            return Result.CreateError<ClientDto>(msg);
        }
        finally
        {
            RequestDataStorage.ClearData(requestId);
        }
    }
}
