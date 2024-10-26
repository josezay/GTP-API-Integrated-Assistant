using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class User
{
    [FirestoreDocumentId]
    public string Id { get; private set; }

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
    public List<Activity> Activities { get; private set; }
    
    [FirestoreProperty]
    public List<Report> Reports { get; private set; }

    [FirestoreProperty]
    public List<Meal> Meals { get; private set; }

    public User()
    {
        Goals = [];
        Activities = [];
        Reports = [];
        Meals = [];
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
        List<Activity> activities)
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
        Activities = activities ?? [];
        Reports = [];
        Meals = [];
    }

    public static User Onboard(
        string name,
        string email,
        string gender,
        int age,
        double height,
        double weight,
        List<Activity> activities)
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
            activities);
    }

    public void AddGoal(Goal goal)
    {
        Goals.Add(goal);
    }
    
    public void AddReport(Report report)
    {
        Reports.Add(report);
    }

    public void AddMeal(Meal meal)
    {
        Meals.Add(meal);
    }
}