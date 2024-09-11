using Hexa_Hub.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly DataContext _context;
        public UserRepo(DataContext context) 
        {
            _context = context;
        }
        public async Task AddUser(User user)
        {
            _context.Users.AddAsync(user);
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new Exception("User Not Found");
            _context.Users.Remove(user);
        }

        public async Task<List<User>> GetAllUser()
        {
            return await _context.Users
                .Include(u => u.AssetAllocations)
                .Include(u => u.ReturnRequests)
                .Include(u => u.AssetRequests)
                .Include(u => u.ServiceRequests)
                .Include(u => u.Audits)
                .Include(u => u.MaintenanceLogs)
                .ToListAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users
                .Include(u => u.AssetAllocations)
                .Include(u => u.ReturnRequests)
                .Include(u => u.AssetRequests)
                .Include(u => u.ServiceRequests)
                .Include(u => u.Audits)
                .Include(u => u.MaintenanceLogs)
                .FirstOrDefaultAsync(u=>u.UserId==id);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public Task<User> UpdateUser(User user)
        {
            _context.Users.Update(user);
            return Task.FromResult(user);
        }

        public async Task<User?> validateUser(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(vu=>vu.UserMail == email &&  vu.Password == password);
        }
    }
}
