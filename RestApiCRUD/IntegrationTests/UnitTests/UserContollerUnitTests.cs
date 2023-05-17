﻿using Xunit;
using RestApi.Controllers;
using AutoMapper;
using RestApi.Interfaces;
using RestApi.Entities;
using RestApi.Profiles;
using Moq;
using System.Collections.Generic;
using System;
using RestApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace UnitTests
{
    public class UserContollerUnitTests
    {
        private IMapper _mapper;
        private List<User> _dbUserList;

        public UserContollerUnitTests()
        {
            _mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new UserControllerMapperProfile())));
            _dbUserList = new List<User>();
            for (int i = 0; i < 5; i++)
            {
                var user = new User
                {
                    EmployeeID = 0,
                    Name = $"TestName{i}",
                    Designation = $"TestDesignation{i}"
                };
                _dbUserList.Add(user);
            }
            _dbUserList.Sort(delegate (User firstUser, User secondUser)
            {
                return firstUser.EmployeeID.CompareTo(secondUser.EmployeeID);
            });
        }

        [Fact]
        public async void GetAll_Ok()
        {
            var mockedUserDbService = CreateMockedUserDbService();
            var controller = new UserController(mockedUserDbService, _mapper);
            var response = await controller.GetAll();
            Assert.True(response is OkObjectResult);
            var responseResult = response as OkObjectResult;
            Assert.True(responseResult.Value is List<UserDto>);
            var responseValue = responseResult.Value as List<UserDto>;
            Assert.NotNull(responseValue);
            Assert.NotEmpty(responseValue);
            Assert.True(Equal(_dbUserList, responseValue));
        }

        //[Fact]
        //public async void GetById_Ok()
        //{
        //    var mockedUserDbService = CreateMockedUserDbService();
        //    var controller = new UserController(mockedUserDbService, _mapper);
        //    foreach (var dbUser in _dbUserList)
        //    {
        //        var response = await controller.GetById(dbUser.EmployeeID);
        //        Assert.True(response is OkObjectResult);
        //        var responseResult = response as OkObjectResult;
        //        Assert.True(responseResult.Value is UserDto);
        //        var responseValue = responseResult.Value as UserDto;
        //        Assert.NotNull(responseValue);
        //        Assert.True(Equal(dbUser, responseValue));
        //    }
        //    GetAll_Ok();
        //}       

        [Fact]
        public async void Create_Created()
        {
            var mockedUserDbService = CreateMockedUserDbService();
            var controller = new UserController(mockedUserDbService, _mapper);
            var userForCreate = new UserForCreateDto
            {
                Name = "CreatedName1",
                Designation = "CreatedDesignation",
            };
            var response = await controller.Create(userForCreate);
            Assert.True(response is CreatedAtActionResult);
            var responseResult = response as CreatedAtActionResult;
            Assert.True(responseResult.Value is UserDto);
            var responseValue = responseResult.Value as UserDto;
            Assert.Equal(userForCreate.Name, responseValue.Name);
            Assert.Equal(userForCreate.Designation, responseValue.Designation);
            _dbUserList.Add(new User
            {
                EmployeeID = responseValue.EmployeeID,
                Name = responseValue.Name,
                Designation = responseValue.Designation,
            });
            GetAll_Ok();
        }

        //[Fact]
        //public async void Delete_Deleted()
        //{
        //    var mockedUserDbService = CreateMockedUserDbService();
        //    var controller = new UserController(mockedUserDbService, _mapper);
        //    var userForDelete = _dbUserList.First();
        //    var response = await controller.Delete(userForDelete.EmployeeID);
        //    Assert.True(response is OkObjectResult);
        //    var responseResult = response as OkObjectResult;
        //    Assert.True(responseResult.Value is UserDto);
        //    var responseValue = responseResult.Value as UserDto;
        //    Assert.True(Equal(userForDelete, responseValue));
        //    _dbUserList.Remove(userForDelete);
        //    GetAll_Ok();
        //}
      

        [Fact]
        public async void Update_Updated()
        {
            var mockedUserDbService = CreateMockedUserDbService();
            var controller = new UserController(mockedUserDbService, _mapper);
            var userIdForUpdate = _dbUserList.First().EmployeeID;
            var userForUpdate = new UserForUpdateDto
            {
                Name = "UpdatedName1",
                Designation = "UpdatedDesignation",
            };
            var response = await controller.Update(userIdForUpdate, userForUpdate);
            Assert.True(response is OkObjectResult);
            var responseResult = response as OkObjectResult;
            Assert.True(responseResult.Value is UserDto);
            var responseValue = responseResult.Value as UserDto;
            Assert.True(Equal(new User
            {
                EmployeeID = userIdForUpdate,
                Name = userForUpdate.Name,
                Designation = userForUpdate.Designation,
            }, responseValue));
            GetAll_Ok();
        }
            

        private bool Equal(User user, UserDto userDto)
        {
            return user.EmployeeID == userDto.EmployeeID && user.Name == userDto.Name && user.Designation == userDto.Designation;
        }

        private bool Equal(List<User> userList, List<UserDto> userDtoList)
        {
            if (userList.Count != userDtoList.Count) return false;
            for (int i = 0; i < userList.Count; i++)
                if (!Equal(userList[i], userDtoList[i])) return false;
            return true;
        }

        private IUserDbService CreateMockedUserDbService()
        {
            var mockUserDbService = new Mock<IUserDbService>();
            mockUserDbService.Setup(service => service.GetAll()).ReturnsAsync(_dbUserList);
            foreach (var user in _dbUserList)
            {
                mockUserDbService.Setup(service => service.GetById(user.EmployeeID)).ReturnsAsync(user);
            }
            mockUserDbService.Setup(service => service.Create(It.IsAny<User>())).ReturnsAsync((User user) => user);
            mockUserDbService.Setup(service => service.Delete(It.IsAny<User>())).ReturnsAsync((User user) => user);
            mockUserDbService.Setup(service => service.Update(It.IsAny<User>())).ReturnsAsync((User user) => user);
            return mockUserDbService.Object;
        }
    }
}