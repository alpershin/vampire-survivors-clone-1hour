using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    public BalanceConfig Balance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadBalance();
    }

    private void LoadBalance()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "balance.json");

        if (!File.Exists(path))
        {
            Debug.LogError($"Balance config not found at {path}");
            Balance = new BalanceConfig();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            Balance = JsonUtility.FromJson<BalanceConfig>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load balance config: {ex.Message}");
            Balance = new BalanceConfig();
        }
    }

    public EnemyStats GetEnemyStats(string type)
    {
        if (Balance?.EnemyStats == null)
        {
            Debug.LogError("EnemyStats not loaded");
            return null;
        }

        return Balance.EnemyStats.FirstOrDefault(e => string.Equals(e.Type, type, StringComparison.OrdinalIgnoreCase));
    }

    public AbilityStats GetAbilityStats(string name)
    {
        if (Balance?.AbilityStats == null)
        {
            Debug.LogError("AbilityStats not loaded");
            return null;
        }

        return Balance.AbilityStats.FirstOrDefault(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
