using Microsoft.EntityFrameworkCore;
using RestApi.DbContexts;
using RestApi.Dtos;
using RestApi.Entities;
using RestApi.Interfaces;

namespace RestApi.Services
{
    public class UserDbService : IUserDbService
    {
        private readonly UserDbContext _userDbContext;

        public UserDbService(UserDbContext userDbContext)
        {
            this._userDbContext = userDbContext;
        }

        public async Task<List<User>> GetAll()
        {            
            var userList =await _userDbContext.Employee.ToListAsync();
            userList.Sort(delegate(User firstUser, User secondUser)
            {
                return firstUser.EmployeeID.CompareTo(secondUser.EmployeeID);
            });
            return userList;
        }    

        public async Task<User> Create(User user)
        {
            _userDbContext.Employee.Add(user);
            var saved = await _userDbContext.SaveChangesAsync();
            if (saved == 0) return null;
            return user;
        }

        public async Task<User> GetById(int id)
        {
            var user = await _userDbContext.Employee.FindAsync(id);
            return user;
        }

        public async Task<User> Delete(User user)
        {
            var deletedUser = new User
            {
                EmployeeID = user.EmployeeID,
                Name = user.Name,
                Designation = user.Designation,
            };
            _userDbContext.Employee.Remove(user);
            var saved = await _userDbContext.SaveChangesAsync();
            if (saved == 0) return null;
            return deletedUser;
        }

        public async Task<User> Update(User user)
        {
            _userDbContext.Employee.Update(user);
            var saved = await _userDbContext.SaveChangesAsync();
            if (saved == 0) return null;
            return user;
        }
    }
}
