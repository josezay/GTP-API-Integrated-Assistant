using Google.Cloud.Firestore;

namespace CompanionAPI.Models;

[FirestoreData]
public class User
{
    [FirestoreProperty]
    public string Name { get; set; }
}