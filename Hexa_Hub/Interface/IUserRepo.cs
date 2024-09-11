using Hexa_Hub.Repository;

namespace Hexa_Hub.Interface
{
    public interface IUserRepo
    {
        Task<List<User>> GetAllUser();
        Task<User?> GetUserById(int id);
        Task AddUser(User user);
        Task<User> UpdateUser(User user);
        Task DeleteUser(int id);
        Task Save();
        Task<User?> validateUser(string email, string password);
        
    }
}
