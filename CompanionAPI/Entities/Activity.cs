using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class Activity
{

    [FirestoreProperty]
    public string Name { get; private set; }

    [FirestoreProperty]
    public double CaloriesBurned { get; private set; }

    [FirestoreProperty]
    public int DurationInMinutes { get; private set; }

    [FirestoreProperty]
    public DateTime CreatedAt { get; private set; }

    public Activity(){}

    private Activity(
        string activityName,
        double caloriesBurned,
        int durationInMinutes,
        DateTime createdAt)
    {
        Name = activityName;
        CaloriesBurned = caloriesBurned;
        DurationInMinutes = durationInMinutes;
        CreatedAt = createdAt;
    }

    public static Activity Create(
        string activityName,
        double caloriesBurned,
        int durationInMinutes)
    {
        return new Activity(
            activityName,
            caloriesBurned,
            durationInMinutes,
            DateTime.UtcNow);
    }
}