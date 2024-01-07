using Users.Api.DataTransferObjects;
using Users.Api.Models;

namespace Users.Api.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync(CancellationToken cancellationToken =default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken =default);
        Task<bool> CreateAsync(UserDtoForInsertion userDtoForInsertion, CancellationToken cancellationToken =default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken =default);
    }
}
