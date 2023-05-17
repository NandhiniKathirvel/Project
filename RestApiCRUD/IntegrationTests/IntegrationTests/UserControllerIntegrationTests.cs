//using Newtonsoft.Json;
//using RestApi.Entities;
//using RestApi.Dtos;
//using RestApi.DbContexts;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using Xunit;
//using System.Net;
//using System.Text;
//using Microsoft.AspNetCore.Mvc.Testing;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using RestApi.Interfaces;
//using RestApi.Services;

//namespace IntegrationTests
//{
//    public class UserControllerIntegrationTests
//    {
//        private readonly HttpClient _client;
//        private readonly List<User> _dbUserList;

//        public UserControllerIntegrationTests()
//        {
//            _dbUserList = new List<User>();
//            var webApp = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
//            {
//                builder.ConfigureServices(services =>
//                {
//                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<UserDbContext>));
//                    if (descriptor != null) services.Remove(descriptor);
//                    services.AddDbContext<UserDbContext>(options => options.UseInMemoryDatabase("Project"));
//                    var dbContext = services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<UserDbContext>();
//                    dbContext.Database.EnsureDeleted();
//                    for (int i = 0; i < 5; i++)
//                    {
//                        var userForDbContext = new User
//                        {
//                            EmployeeID = 0,                           
//                            Name = $"TestName{i}",
//                            Designation = $"TestDesignation{i}",
//                        };
//                        var userForDbUserList = new User
//                        {
//                            EmployeeID = userForDbContext.EmployeeID,
//                            Name = userForDbContext.Name,
//                            Designation = userForDbContext.Designation,
//                        };
//                        dbContext.Add(userForDbContext);
//                        _dbUserList.Add(userForDbUserList);
//                    }
//                    _dbUserList.Sort(delegate (User firstUser, User secondUser)
//                    {
//                        return firstUser.EmployeeID.CompareTo(secondUser.EmployeeID);
//                    });
//                    dbContext.SaveChanges();
//                });
//            });
//            _client = webApp.CreateClient();
//        }

//        [Fact]
//        public async void GetAll_Ok()
//        {
//            var response = await _client.GetAsync("/api/User");
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//            var responseContent = await response.Content.ReadAsStringAsync();
//            Assert.NotNull(responseContent);
//            var userList = JsonConvert.DeserializeObject<List<UserDto>>(responseContent);
//            Assert.NotNull(userList);
//            Assert.NotEmpty(userList);
//            Assert.True(Equal(_dbUserList, userList));
//        }

//        //[Fact]
//        //public async void GetById_Ok()
//        //{
//        //    foreach (var dbUser in _dbUserList)
//        //    {
//        //        var response = await _client.GetAsync($"/api/User/{dbUser.EmployeeID}");
//        //        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        //        var responseContent = await response.Content.ReadAsStringAsync();
//        //        Assert.NotNull(responseContent);
//        //        var user = JsonConvert.DeserializeObject<UserDto>(responseContent);
//        //        Assert.NotNull(user);
//        //        Assert.True(Equal(dbUser, user));
//        //    }
//        //    GetAll_Ok();
//        //}
       
//        [Fact]
//        public async void Create_Created()
//        {
//            var userForCreate = new UserForCreateDto
//            {
//                Name = "CreatedName1",
//                Designation = "CreatedDesignation",
//            };
//            var requestContent = new StringContent(JsonConvert.SerializeObject(userForCreate), Encoding.UTF8, "application/json");
//            var response = await _client.PostAsync("/api/User", requestContent);
//            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//            var responseContent = await response.Content.ReadAsStringAsync();
//            Assert.NotNull(responseContent);
//            var user = JsonConvert.DeserializeObject<UserDto>(responseContent);
//            Assert.NotNull(user);
//            Assert.Equal(userForCreate.Name, user.Name);
//            Assert.Equal(userForCreate.Designation, user.Designation);
//            _dbUserList.Add(new User
//            {
//                EmployeeID = user.EmployeeID,
//                Name = user.Name,
//                Designation = user.Designation,
//            });
//            _dbUserList.Sort(delegate (User firstUser, User secondUser)
//            {
//                return firstUser.EmployeeID.CompareTo(secondUser.EmployeeID);
//            });
//            GetAll_Ok();
//        }        

//        //[Fact]
//        //public async void Delete_Deleted()
//        //{
//        //    var userForDelete = _dbUserList.First();
//        //    var response = await _client.DeleteAsync($"/api/User/{userForDelete.EmployeeID}");
//        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        //    var responseContent = await response.Content.ReadAsStringAsync();
//        //    Assert.NotNull(responseContent);
//        //    var user = JsonConvert.DeserializeObject<UserDto>(responseContent);
//        //    Assert.NotNull(user);
//        //    Assert.True(Equal(userForDelete, user));
//        //    _dbUserList.Remove(userForDelete);
//        //    _dbUserList.Sort(delegate (User firstUser, User secondUser)
//        //    {
//        //        return firstUser.EmployeeID.CompareTo(secondUser.EmployeeID);
//        //    });
//        //    GetAll_Ok();
//        //}
      

//        [Fact]
//        public async void Update_Updated()
//        {
//            var dbUser = _dbUserList.First();
//            var userForUpdate = new UserForCreateDto
//            {
//                Name = "UpdatedName1",
//                Designation = "UpdatedDesignation",
//            };
//            var userShouldBe = new User
//            {
//                EmployeeID = dbUser.EmployeeID,
//                Name = userForUpdate.Name,
//                Designation = userForUpdate.Designation,
//            };
//            var requestContent = new StringContent(JsonConvert.SerializeObject(userForUpdate), Encoding.UTF8, "application/json");
//            var response = await _client.PutAsync($"/api/User/{dbUser.EmployeeID}", requestContent);
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//            var responseContent = await response.Content.ReadAsStringAsync();
//            Assert.NotNull(responseContent);
//            var user = JsonConvert.DeserializeObject<UserDto>(responseContent);
//            Assert.NotNull(user);
//            Assert.True(Equal(userShouldBe, user));
//            dbUser.Name = userForUpdate.Name;
//            dbUser.Designation = userForUpdate.Designation;
//            _dbUserList.Sort(delegate (User firstUser, User secondUser)
//            {
//                return firstUser.EmployeeID.CompareTo(secondUser.EmployeeID);
//            });
//            GetAll_Ok();
//        }
    
//        private bool Equal(User user, UserDto userDto)
//        {
//            return user.EmployeeID == userDto.EmployeeID && user.Name == userDto.Name && user.Designation == userDto.Designation;
//        }

//        private bool Equal(List<User> userList, List<UserDto> userDtoList)
//        {
//            if (userList.Count != userDtoList.Count) return false;
//            for (int i = 0; i < userList.Count; i++)
//                if (!Equal(userList[i], userDtoList[i])) return false;
//            return true;
//        }
//    }
//}