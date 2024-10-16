using CompanionAPI.Models;

namespace CompanionAPI.Repositories.UserRepository;

public interface IUserRepository
{
    Task SaveUserAsync(User user);
}