using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class User
{

    [FirestoreProperty]
    public string Name { get; private set; }

    [FirestoreProperty]
    public string Email { get; private set; }

    [FirestoreProperty]
    public string Gender { get; private set; }

    [FirestoreProperty]
    public int Age { get; private set; }

    [FirestoreProperty]
    public double Height { get; private set; }

    [FirestoreProperty]
    public double Weight { get; private set; }

    [FirestoreProperty]
    public DateTime CreatedAt { get; private set; }

    [FirestoreProperty]
    public bool FirstLogin { get; private set; }

    [FirestoreProperty]
    public List<Goal> Goals { get; private set; }

    [FirestoreProperty]
    public List<Exercise> Exercises { get; private set; }

    public User()
    {
        Goals = [];
        Exercises = [];
    }

    private User(
        string name,
        string email,
        string gender,
        int age,
        double height,
        double weight,
        DateTime createdAt,
        bool firstLogin,
        List<Exercise> exercises)
    {
        Name = name;
        Email = email;
        Gender = gender;
        Age = age;
        Height = height;
        Weight = weight;
        CreatedAt = createdAt;
        FirstLogin = firstLogin;
        Goals = [];
        Exercises = exercises ?? [];
    }

    public static User Onboard(
        string name,
        string email,
        string gender,
        int age,
        double height,
        double weight,
        List<Exercise> exercises)
    {
        return new User(
            name,
            email,
            gender,
            age,
            height,
            weight,
            DateTime.UtcNow,
            true,
            exercises);
    }

    public void AddGoal(Goal goal)
    {
        Goals.Add(goal);
    }
}