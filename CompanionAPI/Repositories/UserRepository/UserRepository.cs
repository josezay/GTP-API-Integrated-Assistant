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

    public async Task<User> SaveUserAsync(User user)
    {
        CollectionReference collection = _firestoreDb.Collection("users");
        DocumentReference documentReference = await collection.AddAsync(user);
        DocumentSnapshot documentSnapshot = await documentReference.GetSnapshotAsync();
        return documentSnapshot.ConvertTo<User>();
    }

    public async Task UpdateUserAsync(User user)
    {
        DocumentReference document = _firestoreDb.Collection("users").Document(user.Id);
        await document.SetAsync(user, SetOptions.Overwrite);
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

    public async Task<User?> GetUserByIdAsync(string id)
    {
        DocumentReference document = _firestoreDb.Collection("users").Document(id);
        DocumentSnapshot snapshot = await document.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            return snapshot.ConvertTo<User>();
        }

        return null;
    }
}
