using CompanionAPI.Entities;
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

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        CollectionReference collection = _firestoreDb.Collection("users");
        Query query = collection.WhereEqualTo("Email", email);
        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        if (snapshot.Documents.Count > 0)
        {
            DocumentSnapshot document = snapshot.Documents[0];
            return document.ConvertTo<User>();
        }

        return null;
    }
}
