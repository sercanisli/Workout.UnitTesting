using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Services;

namespace Users.Api.Tests.Unit.ServicesTests
{
    public class UserServiceTests
    {
        private readonly UserManager _sut;
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly ILogger<User> _logger = Substitute.For<ILogger<User>>();
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
    }
}
