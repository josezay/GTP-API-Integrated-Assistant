using CompanionAPI.Entities;

namespace CompanionAPI.Repositories.UserRepository;

public interface IUserRepository
{
    Task SaveUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
}