using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class DailySummary
{
    [FirestoreProperty]
    public DateTime Date { get; set; }
    
    [FirestoreProperty]
    public double TotalCaloriesConsumed { get; set; }
    
    [FirestoreProperty]
    public double TotalProteinsConsumed { get; set; }
    
    [FirestoreProperty]
    public bool CaloriesGoalAchieved { get; set; }
    
    [FirestoreProperty]
    public bool ProteinsGoalAchieved { get; set; }
}
