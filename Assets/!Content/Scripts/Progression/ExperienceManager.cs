using System;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    [SerializeField] private int xpToNextLevel = 100;

    public int currentXp { get; private set; }
    public int level { get; private set; } = 1;

    public event Action<float, float> OnXpChanged;
    public event Action OnLevelUp;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        RaiseXpChanged();
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentXp += amount;
        bool leveledUp = false;
        while (currentXp >= xpToNextLevel)
        {
            currentXp -= xpToNextLevel;
            level++;
            leveledUp = true;
        }

        RaiseXpChanged();
        if (leveledUp)
        {
            OnLevelUp?.Invoke();
        }
    }

    private void RaiseXpChanged()
    {
        OnXpChanged?.Invoke(currentXp, xpToNextLevel);
    }
}
