﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Users.Api.Controllers;
using Users.Api.Models;
using Users.Api.Services;

namespace Users.Api.Tests.Unit.ControllersTests
{
    public class UserControllerTests
    {
        private readonly UsersController _sut;
        private readonly IUserService _userService = Substitute.For<IUserService>();

        public UserControllerTests()
        {
            _sut = new(_userService);
        }

        [Fact]
        public async Task GetAll_ShouldReturnUsers()
        {
            //Arrange
            _userService.GetAllAsync().Returns(Enumerable.Empty<User>().ToList());

            //Act
            var result = (OkObjectResult)await _sut.GetAll(default);

            //Assert
            result.StatusCode.Should().Be(200);
        }
    }
}
