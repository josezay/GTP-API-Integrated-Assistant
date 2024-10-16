using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class Goal
{
    [FirestoreProperty]
    public DateTime CreatedAt { get; private set; }

    [FirestoreProperty]
    public int Calories { get; private set; }

    [FirestoreProperty]
    public int Proteins { get; private set; }

    public Goal() { }

    private Goal(DateTime createdAt, int calories, int proteins)
    {
        CreatedAt = createdAt;
        Calories = calories;
        Proteins = proteins;
    }

    public static Goal Create(int calories, int proteins)
    {
        return new Goal(DateTime.UtcNow, calories, proteins);
    }
}