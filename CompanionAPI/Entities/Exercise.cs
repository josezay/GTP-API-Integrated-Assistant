using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class Exercise
{

    [FirestoreProperty]
    public string ActivityName { get; private set; }

    [FirestoreProperty]
    public int WeeklyFrequency { get; private set; }

    [FirestoreProperty]
    public int DurationInMinutes { get; private set; }

    [FirestoreProperty]
    public DateTime CreatedAt { get; private set; }

    public Exercise(){}

    private Exercise(
        string activityName,
        int weeklyFrequency,
        int durationInMinutes,
        DateTime createdAt)
    {
        ActivityName = activityName;
        WeeklyFrequency = weeklyFrequency;
        DurationInMinutes = durationInMinutes;
        CreatedAt = createdAt;
    }

    public static Exercise Create(
        string activityName,
        int weeklyFrequency,
        int durationInMinutes)
    {
        return new Exercise(
            activityName,
            weeklyFrequency,
            durationInMinutes,
            DateTime.UtcNow);
    }
}