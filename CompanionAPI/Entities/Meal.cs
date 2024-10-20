using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class Meal
{
    [FirestoreProperty]
    public string Name { get; private set; }

    [FirestoreProperty]
    public int Calories { get; private set; }

    [FirestoreProperty]
    public int Proteins { get; private set; }

    [FirestoreProperty]
    public int Quantity { get; private set; }

    [FirestoreProperty]
    public string Unit { get; private set; }

    [FirestoreProperty]
    public DateTime CreatedAt { get; private set; }

    public Meal() { }

    private Meal(
        string name,
        int calories,
        int proteins,
        int quantity,
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
        int calories,
        int proteins,
        int quantity,
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