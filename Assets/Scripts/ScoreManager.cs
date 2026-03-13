using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int Score;
    private int Streak = 0;
    [SerializeField] private int PointsForMatch = 10;
    [SerializeField] private int StreakBonus = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public (int totalScore, int? scoreChange) AddScore(bool isMatch)
    {
        if (!isMatch)
        {
            Streak = 0;
            return (Score, null);
        }

        int scoreChange = PointsForMatch + Mathf.FloorToInt(StreakBonus * Streak++);
        Score += scoreChange;

        return (Score, scoreChange);
    }

    public (int score, int streak) GetScoreAndStreak()
    {
        return (Score, Streak);
    }

    public void SetScoreAndStreak(int score, int streak)
    {
        Score = score;
        Streak = streak;
    }
}
