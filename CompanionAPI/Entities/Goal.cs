using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class Goal
{
    [FirestoreProperty]
    public DateTime Datetime { get; private set; }

    [FirestoreProperty]
    public int Calories { get; private set; }

    [FirestoreProperty]
    public int Proteins { get; private set; }

    public Goal() { }

    private Goal(DateTime datetime, int calories, int proteins)
    {
        Datetime = datetime;
        Calories = calories;
        Proteins = proteins;
    }

    public static Goal Create(int calories, int proteins)
    {
        return new Goal(DateTime.UtcNow, calories, proteins);
    }
}