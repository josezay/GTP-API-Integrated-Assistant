using CompanionAPI.Models;
using Google.Cloud.Firestore;

namespace CompanionAPI.Repositories.UserRepository;

public class UserRepository : IUserRepository
{
    private readonly FirestoreDb _firestoreDb;

    public UserRepository(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task SaveUserAsync(User user)
    {
        CollectionReference collection = _firestoreDb.Collection("users");
        await collection.AddAsync(user);
    }
}
