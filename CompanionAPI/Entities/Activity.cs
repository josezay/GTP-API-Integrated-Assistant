using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class Activity
{

    [FirestoreProperty]
    public string Name { get; private set; }

    [FirestoreProperty]
    public int WeeklyFrequency { get; private set; }

    [FirestoreProperty]
    public int DurationInMinutes { get; private set; }

    [FirestoreProperty]
    public DateTime CreatedAt { get; private set; }

    public Activity(){}

    private Activity(
        string activityName,
        int weeklyFrequency,
        int durationInMinutes,
        DateTime createdAt)
    {
        Name = activityName;
        WeeklyFrequency = weeklyFrequency;
        DurationInMinutes = durationInMinutes;
        CreatedAt = createdAt;
    }

    public static Activity Create(
        string activityName,
        int weeklyFrequency,
        int durationInMinutes)
    {
        return new Activity(
            activityName,
            weeklyFrequency,
            durationInMinutes,
            DateTime.UtcNow);
    }
}