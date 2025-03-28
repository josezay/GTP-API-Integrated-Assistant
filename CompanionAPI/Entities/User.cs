﻿using Google.Cloud.Firestore;

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

    [FirestoreProperty]
    public List<DailySummary> DailySummary { get; private set; }

    [FirestoreProperty]
    public double ActivityFactor { get; private set; }


    public User()
    {
        Goals = [];
        Activities = [];
        Reports = [];
        Meals = [];
        DailySummary = [];
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
        List<Activity> activities,
        double activityFactor)
    {
        Name = name;
        Email = email;
        Gender = gender;
        Age = age;
        Height = height;
        Weight = weight;
        CreatedAt = createdAt;
        FirstLogin = firstLogin;
        ActivityFactor = activityFactor;
        Goals = [];
        Activities = activities ?? [];
        Reports = [];
        Meals = [];
        DailySummary = [];
    }

    public static User Onboard(
        string name,
        string email,
        string gender,
        int age,
        double height,
        double weight,
        List<Activity> activities,
        double activityFactor)
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
            activities,
            activityFactor);
    }

    public void AddGoal(Goal goal)
    {
        Goals.Add(goal);
    }
    
    public void AddReport(Report report)
    {
        Reports.Add(report);
    }

    private void UpdateDailySummary(double calories, double proteins)
    {
        var today = DateTime.UtcNow.Date;
        var summary = DailySummary.FirstOrDefault(ds => ds.Date == today);

        if (summary == null)
        {
            summary = new DailySummary
            {
                Date = today,
                TotalCaloriesConsumed = calories,
                TotalProteinsConsumed = proteins,
                CaloriesGoalAchieved = false,
                ProteinsGoalAchieved = false
            };

            DailySummary.Add(summary);
        }
        else
        {
            summary.TotalCaloriesConsumed += calories;
            summary.TotalProteinsConsumed += proteins;
        }

        // TODO: adicionar a lógica para verificar se as metas de calorias e proteínas foram atingidas
    }


    public void AddMeal(Meal meal)
    {
        Meals.Add(meal);
        UpdateDailySummary(meal.Calories, meal.Proteins);
    }

    public void AddActivity(Activity activity)
    {
        Activities.Add(activity);
        UpdateDailySummary(activity.CaloriesBurned * (-1), 0);
    }

    public void UpdateWeight(double weight)
    {
        Weight = weight;
    }
}