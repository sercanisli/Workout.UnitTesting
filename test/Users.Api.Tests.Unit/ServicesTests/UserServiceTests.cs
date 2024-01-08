using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
    }
}
