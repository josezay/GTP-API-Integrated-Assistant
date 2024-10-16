using Google.Cloud.Firestore;

namespace CompanionAPI.Models;

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

    public User()
    {
        Goals = [];
    }

    private User(
        string name,
        string email,
        string gender,
        int age,
        double height,
        double weight,
        DateTime createdAt,
        bool firstLogin)
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
    }

    public static User Onboard(
        string name,
        string email,
        string gender,
        int age,
        double height,
        double weight)
    {
        return new User(
            name,
            email,
            gender,
            age,
            height,
            weight,
            DateTime.UtcNow,
            true);
    }

    public void AddGoal(Goal goal)
    {
        Goals.Add(goal);
    }
}