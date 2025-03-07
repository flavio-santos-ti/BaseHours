using BaseHours.Application.Dtos;
using FDS.NetCore.ApiResponse.Models;

namespace BaseHours.Application.Interfaces;

public interface IClientService
{
    Task<Response<ClientDto>> AddAsync(ClientRequestDto request);
    Task<Response<bool>> DeleteAsync(Guid id);
    Task<Response<IEnumerable<ClientDto>>> GetAllAsync();
    Task<Response<ClientDto>> GetByIdAsync(string id);
    Task<Response<IEnumerable<ClientDto>>> SearchByNameAsync(string name);
    Task<Response<ClientDto>> UpdateAsync(ClientDto clientDto);
}
