using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Users.Api.DataTransferObjects;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Utilities.FluentValidation;

namespace Users.Api.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync(CancellationToken cancellationToken =default);
        Task<User?> GetByIdAsync(Guid guid, CancellationToken cancellationToken =default);
        Task<bool> CreateAsync(UserDtoForInsertion userDtoForInsertion, CancellationToken cancellationToken =default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken =default);
    }

    public sealed class UserManager(IUserRepository _repository, ILogger<User> _logger, IMapper _mapper) : IUserService
    {
        public async Task<bool> CreateAsync(UserDtoForInsertion userDtoForInsertion, CancellationToken cancellationToken = default)
        {
            UserDtoForValidation validationRules = new UserDtoForValidation();
            var result = validationRules.Validate(userDtoForInsertion);
            if(!result.IsValid)
            {
                throw new ValidationException(string.Join(", ", result.Errors.Select(e=>e.ErrorMessage)));
            }
            _logger.LogInformation("Creating user yith id {0} and name {1}", userDtoForInsertion.Id, userDtoForInsertion.FullName);
            var mappedUser = _mapper.Map<User>(userDtoForInsertion);
            var stopWatch = Stopwatch.StartNew();
            try
            {
                return await _repository.CreateAsync(mappedUser, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while creating a user");
            }
            finally
            {
                stopWatch.Stop();
                _logger.LogInformation("User with id: {0} created in {1}ms", mappedUser.Id, stopWatch.ElapsedMilliseconds);
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            User user = await _repository.GetByIdAsync(id, cancellationToken);
            if(user == null)
            {
                throw new ArgumentException($"User with id : {id} is not found");
            }

        }

        public Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(Guid guid, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
