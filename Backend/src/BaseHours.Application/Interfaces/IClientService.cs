using BaseHours.Application.Dtos;
using FDS.NetCore.ApiResponse.Models;

namespace BaseHours.Application.Interfaces;

public interface IClientService
{
    Task<Response<ClientDto>> AddAsync(ClientRequestDto request);
    Task<Response<bool>> DeleteAsync(Guid id);
    Task<IEnumerable<ClientDto>> GetAllAsync();
    Task<ClientDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ClientDto>> SearchByNameAsync(string name);
    Task UpdateAsync(ClientDto clientDto);
}
