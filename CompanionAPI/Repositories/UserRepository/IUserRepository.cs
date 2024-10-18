using CompanionAPI.Entities;

namespace CompanionAPI.Repositories.UserRepository;

public interface IUserRepository
{
    Task SaveUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(string id);
}