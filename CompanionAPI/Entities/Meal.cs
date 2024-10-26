using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class Meal
{
    [FirestoreProperty]
    public string Name { get; private set; }

    [FirestoreProperty]
    public double Calories { get; private set; }

    [FirestoreProperty]
    public double Proteins { get; private set; }

    [FirestoreProperty]
    public double Quantity { get; private set; }

    [FirestoreProperty]
    public string Unit { get; private set; }

    [FirestoreProperty]
    public DateTime CreatedAt { get; private set; }

    public Meal() { }

    private Meal(
        string name,
        double calories,
        double proteins,
        double quantity,
        string unit,
        DateTime createdAt)
    {
        Name = name;
        Calories = calories;
        Proteins = proteins;
        Quantity = quantity;
        Unit = unit;
        CreatedAt = createdAt;
    }

    public static Meal Create(
        string name,
        double calories,
        double proteins,
        double quantity,
        string unit)
    {
        DateTime createdAt = DateTime.UtcNow;

        return new Meal(
            name,
            calories,
            proteins,
            quantity,
            unit,
            createdAt
        );
    }
}