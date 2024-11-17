using CompanionAPI.Entities;
using Google.Api;
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

    public async Task<List<Report>> GetReportsFromLastThreeDaysAsync(string userId)
    {
        DocumentReference userDocument = _firestoreDb.Collection("users").Document(userId);
        DocumentSnapshot userSnapshot = await userDocument.GetSnapshotAsync();

        if (!userSnapshot.Exists)
        {
            return new List<Report>();
        }

        User user = userSnapshot.ConvertTo<User>();
        var threeDaysAgo = DateTime.UtcNow.AddDays(-3);

        List<Report> recentReports = user.Reports
            .Where(report => report.QueriedAt >= threeDaysAgo)
            .ToList();

        return recentReports;
    }
}
