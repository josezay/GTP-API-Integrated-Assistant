using Google.Cloud.Firestore;

namespace CompanionAPI.Entities;

[FirestoreData]
public class Report
{
    [FirestoreProperty]
    public string Query { get; private set; }

    [FirestoreProperty]
    public DateTime QueriedAt { get; private set; }

    [FirestoreProperty]
    public string Result { get; private set; }

    [FirestoreProperty]
    public bool IsSuccessful { get; private set; }

    public Report()
    {
    }

    private Report(
        string query,
        DateTime queriedAt)
    {
        Query = query;
        QueriedAt = queriedAt;
    }

    public static Report Create(
        string query)
    {
        var queriedAt = DateTime.UtcNow;

        return new Report(
            query,
            queriedAt);
    }

}