﻿using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Users.Api.DataTransferObjects;
using Users.Api.Logging;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Services;

namespace Users.Api.Tests.Unit.ServicesTests
{
    public class UserServiceTests
    {
        private readonly UserManager _sut;
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly ILoggerAdapter<UserManager> _logger = Substitute.For<ILoggerAdapter<UserManager>>();
        private readonly IMapper _mapper;

        public UserServiceTests()
        {
            _sut = new(_userRepository, _logger, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUserExist()
        {
            //Arrange
            _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());

            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnsUsers_WhenSomeUsersExist()
        {
            //Arrange
            var testUser = new User() 
            {
                Id = Guid.NewGuid(),
                FullName = "Sercan ISLI"
            };
            var expectedUser = new List<User>()
            {
                testUser
            };

            _userRepository.GetAllAsync().Returns(expectedUser);
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetAllAsync_ShouldLogMessages_WhenInvoked()
        {
            //Arrange
            _userRepository.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());

            //Act
            await _sut.GetAllAsync();

            //Assert
            _logger.Received(1).LogInformation(Arg.Is<string>(s => s.Contains("Retrieving all users")));
            _logger.Received(1).LogInformation(Arg.Is<string>(s => s.Contains("All users retrieved")), Arg.Any<object[]>());

        }

        [Fact]
        public async Task GetAllAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
        {
            //Arrange
            var exception = new ArgumentException("Something went wrong while retrieving all users");
            _userRepository.GetAllAsync().Throws(exception);

            //Act
            var requestAction = async () => await _sut.GetAllAsync();

            //Assert
            await requestAction.Should().ThrowAsync<ArgumentException>();
            _logger.Received(1).LogError(Arg.Is(exception), Arg.Is("Something went wrong while retrieving all users"));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNoUserExists()
        {
            //Arrange
            _userRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

            //Act
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            //Assert
           result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenSomeUserExists()
        {
            //Arrange
            var existingUser = new User()
            {
                Id = Guid.NewGuid(),
                FullName = "Sercan ISLI"
            };

            _userRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(existingUser);
            //Act
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            //Assert
            result.Should().BeEquivalentTo(existingUser);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldLogMessages_WhenInvoked()
        {
            //Arrange
            var userId = Guid.NewGuid();
            _userRepository.GetByIdAsync(userId).ReturnsNull();

            //Act
            await _sut.GetByIdAsync(userId);

            //Assert
            _logger.Received(1).LogInformation(Arg.Is<string>(s => s.Contains("Retrieving user with id :")));
            _logger.Received(1).LogInformation(Arg.Is<string>(s => s.Contains($"User with id : {userId} retrieved in")), Arg.Any<object[]>());

        }

        [Fact]
        public async Task GetByIdAsync_ShouldLogMessageAndException_WhenExceptionIsThrown()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var exception = new ArgumentException("Something went wrong while retrieving user");
            _userRepository.GetByIdAsync(userId).Throws(exception);

            //Act
            var requestAction = async () => await _sut.GetByIdAsync(userId);

            //Assert
            await requestAction.Should().ThrowAsync<ArgumentException>();
            _logger.Received(1).LogError(Arg.Is(exception), Arg.Is($"Something went wrong while retrieving user with id : {userId}"));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrownAnError_WhenUserCreateDetailsAreNotValid()
        {
            //Arrange
            UserDtoForInsertion userDtoForInsertion = new UserDtoForInsertion()
            {
                FullName = ""
            };

            //Act
            var action = async () => await _sut.CreateAsync(userDtoForInsertion); 

            //Assert
            action.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrownAnError_WhenUserNameExists()
        {
            //Arrange
            _userRepository.IsNameExist(Arg.Any<string>()).Returns(true);

            //Act
            var action = async () => await _sut.CreateAsync(new UserDtoForInsertion()
            {
                FullName = "Sercan ISLI"
            });

            //Assert
            action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public void CreateAsync_ShouldUserDtoForInseritonToUserObject()
        {
            //Arrange
            UserDtoForInsertion userDtoForInsertion = new UserDtoForInsertion()
            {
                FullName = "Sercan ISLI"
            };

            //Act
            var user = _sut.UserDtoForInseritonToUserObject(userDtoForInsertion);

            //Assert
            user.FullName.Should().Be(userDtoForInsertion.FullName);
        }

        [Fact]
        public async void CreateAsync_ShouldCreateUser_WhenDetailsAreValidAndUnique()
        {
            //Arrange
            UserDtoForInsertion userDtoForInsertion = new UserDtoForInsertion()
            {
                FullName = "Sercan ISLI"
            };

            _userRepository.IsNameExist(userDtoForInsertion.FullName).Returns(false);
            _userRepository.CreateAsync(Arg.Any<User>()).Returns(true);

            //Act
            var result = await _sut.CreateAsync(userDtoForInsertion);

            //Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task CreateAsync_ShouldLogMessages_WhenIvoked()
        {
            //Arrange
            UserDtoForInsertion userDtoForInsertion = new UserDtoForInsertion()
            {
                FullName = "Sercan ISLI"
            };

            _userRepository.IsNameExist(userDtoForInsertion.FullName).Returns(false);
            _userRepository.CreateAsync(Arg.Any<User>()).Returns(true);

            //Act
            await _sut.CreateAsync(userDtoForInsertion);

            //Assert
            _logger.Received(1).LogInformation(Arg.Is<string>(s => s.Contains($"User with id: {userDtoForInsertion.Id} created in")));
            _logger.Received(1).LogInformation(Arg.Is<string>(s => s.Contains($"Creating user yith id {userDtoForInsertion.Id} and name {userDtoForInsertion.FullName}")), Arg.Any<object[]>());
        }

        [Fact]
        public async Task CreateAsync_ShouldLogMessagesAndExceptions_WhenExcceptionIsThrown()
        {
            //Arrange
            UserDtoForInsertion userDtoForInsertion = new UserDtoForInsertion()
            {
                FullName = "Sercan ISLI"
            };

            var exception = new ArgumentException("Something went wrong while creating a user");
            _userRepository.CreateAsync(Arg.Any<User>()).Throws(exception);

            //Act
           var action = async () => await _sut.CreateAsync(userDtoForInsertion);

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();

            _logger.Received(1).LogError(Arg.Is(exception), Arg.Is("Something went wrong while creating a user"));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrownAnError_WhenUserNoExist()
        {
            //Arrange
            var userId = Guid.NewGuid();

            _userRepository.GetByIdAsync(userId).ReturnsNull();

            //Act
            var action = async () => await _sut.DeleteAsync(userId);

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUser_WhenUserExist()
        {
            //Arrange
            var userId = Guid.NewGuid();

            User user = new User()
            {
                Id = userId,
                FullName = "Sercan ISLI"
            };
            _userRepository.GetByIdAsync(userId).Returns(user);
            _userRepository.DeleteAsync(user).Returns(true);

            //Act
            var result = await _sut.DeleteAsync(userId);

            //Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task DeleteAsync_ShouldLogMessages_WhenIvoked()
        {
            //Arrange
            var userId = Guid.NewGuid();
            User user = new User()
            {
                Id = userId,
                FullName = "Sercan ISLI"
            };

            _userRepository.GetByIdAsync(userId).Returns(user);
            _userRepository.DeleteAsync(user).Returns(true);

            //Act
            await _sut.DeleteAsync(userId);

            //Assert
            _logger.Received(1).LogInformation(Arg.Is<string>(s => s.Contains($"Deleting user with id : {userId}")));
            _logger.Received(1).LogInformation(Arg.Is<string>(s => s.Contains($"User with id : {userId} deleted in")), Arg.Any<object[]>());
        }

        [Fact]
        public async Task DeleteAsync_ShouldLogMessagesAndExceptions_WhenExcceptionIsThrown()
        {
            //Arrange
            var userId = Guid.NewGuid();
            User user = new User()
            {
                Id = userId,
                FullName = "Sercan ISLI"
            };

            _userRepository.GetByIdAsync(userId).Returns(user);
            var exception = new ArgumentException("Something went wrong while deleting user");
            _userRepository.DeleteAsync(user).Throws(exception);

            //Act
            var action = async () => await _sut.DeleteAsync(userId);

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();

            _logger.Received(1).LogError(Arg.Is(exception), Arg.Is("Something went wrong while deleting user"));
        }
    }
}
