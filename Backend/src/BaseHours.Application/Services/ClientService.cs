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

            return Result.CreateDelete<bool>("Client deleted successfully.");
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
        try
        {
            var clients = await _clientRepository.GetAllAsync();
            var clientDtos = clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });

            return Result.CreateRead<IEnumerable<ClientDto>>(clients.Any() ? "Clients retrieved successfully." : "No clients found.", clientDtos);
        }
        catch (Exception ex)
        {
            return Result.CreateError<IEnumerable<ClientDto>>($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Response<ClientDto>> GetByIdAsync(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out Guid validGuid))
            {
                return Result.CreateValidationError<ClientDto>("Invalid client ID format.");
            }

            var client = await _clientRepository.GetByIdAsync(validGuid);

            if (client is null)
            {
                return Result.CreateNotFound<ClientDto>("Client not found.");
            }

            var clientDto = new ClientDto { Id = client.Id, Name = client.Name };

            return Result.CreateRead<ClientDto>("Client retrieved successfully.", clientDto);
        }
        catch (Exception ex)
        {
            return Result.CreateError<ClientDto>($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Response<IEnumerable<ClientDto>>> SearchByNameAsync(string name)
    {
        try
        {
            var clients = await _clientRepository.SearchByNameAsync(name);
            var clientDtos = clients.Select(c => new ClientDto { Id = c.Id, Name = c.Name });

            return Result.CreateRead<IEnumerable<ClientDto>>(clients.Any() ? "Clients retrieved successfully." : "No clients found with the given name.", clientDtos);
        }
        catch (Exception ex)
        {
            return Result.CreateError<IEnumerable<ClientDto>>($"An unexpected error occurred: {ex.Message}");
        }  
    }

    public async Task<Response<ClientDto>> UpdateAsync(ClientDto clientDto)
    {
        try
        {
            var existingClient = await _clientRepository.GetByIdAsync(clientDto.Id);
            if (existingClient is null)
            {
                return Result.CreateNotFound<ClientDto>("Client not found.");
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
            return Result.CreateError<ClientDto>($"An unexpected error occurred: {ex.Message}");
        }
    }
}
