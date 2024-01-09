using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Users.Api.DataTransferObjects;
using Users.Api.Logging;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Utilities.FluentValidation;

namespace Users.Api.Services
{
    public sealed class UserManager(IUserRepository _repository, ILoggerAdapter<UserManager> _logger, IMapper _mapper) : IUserService
    {
        public async Task<bool> CreateAsync(UserDtoForInsertion userDtoForInsertion, CancellationToken cancellationToken = default)
        {
            UserDtoForValidation validationRules = new UserDtoForValidation();
            var result = validationRules.Validate(userDtoForInsertion);
            if(!result.IsValid)
            {
                throw new ValidationException(string.Join(", ", result.Errors.Select(e=>e.ErrorMessage)));
            }
            var isNameExist = await _repository.IsNameExist(userDtoForInsertion.FullName,cancellationToken);
            if (isNameExist)
            {
                throw new ArgumentException("Name already exist ");
            }
            var user = UserDtoForInseritonToUserObject(userDtoForInsertion);
            _logger.LogInformation($"Creating user yith id {userDtoForInsertion.Id} and name {userDtoForInsertion.FullName}");
            var stopWatch = Stopwatch.StartNew();
            try
            {
                return await _repository.CreateAsync(user, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while creating a user");
                throw;
            }
            finally
            {
                stopWatch.Stop();
                _logger.LogInformation($"User with id: {user.Id} created in {stopWatch.ElapsedMilliseconds}ms");
            }
            
        }

        public User UserDtoForInseritonToUserObject(UserDtoForInsertion userDtoForInsertion)
        {
            User user = new User()
            {
                FullName = userDtoForInsertion.FullName,
            };
            return user;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            User? user = await _repository.GetByIdAsync(id, cancellationToken);
            if(user == null)
            {
                throw new ArgumentException($"User with id : {id} is not found");
            }
            _logger.LogInformation($"Deleting user with id : {id}");
            var stopWath = Stopwatch.StartNew();
            try
            {
                return await _repository.DeleteAsync(user, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while deleting user");
                throw;
            }
            finally
            {
                stopWath.Stop();
                _logger.LogInformation($"User with id : {id} deleted in {stopWath.ElapsedMilliseconds}ms");
            }
        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all users");
            var stopWatch = Stopwatch.StartNew();

            try
            {
                return await _repository.GetAllAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while retrieving all users");
                throw;
            }
            finally
            {
                stopWatch.Stop();
                _logger.LogInformation($"All users retrieved in {stopWatch.ElapsedMilliseconds} ms");
            }
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Retrieving user with id : {id}");
            var stopWatch = Stopwatch.StartNew();

            try
            {
                return await _repository.GetByIdAsync(id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while retrieving user with id : {id}");
                throw;
            }
            finally
            {
                stopWatch.Stop();
                _logger.LogInformation($"User with id : {id} retrieved in {stopWatch.ElapsedMilliseconds}ms");
            }
        }
    }
}
