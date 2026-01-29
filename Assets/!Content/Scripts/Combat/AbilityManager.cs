using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityOption
{
    public string Name;
    public string Description;
    public int CurrentLevel;
    public int NextLevel;
    public bool IsNew;
}

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private ProjectileAbility projectileAbility;
    [SerializeField] private AuraAbility auraAbility;
    [SerializeField] private int maxLevelPerAbility = 5;

    private readonly Dictionary<string, AbilityState> abilityStates = new Dictionary<string, AbilityState>();

    private class AbilityState
    {
        public string Name;
        public BaseAbility Component;
        public int Level;
        public bool Unlocked => Level > 0;
    }

    private void Awake()
    {
        RegisterAbility("Projectile", projectileAbility);
        RegisterAbility("Aura", auraAbility);

        // Disable all ability components until configured via unlock/upgrade
        if (projectileAbility != null) projectileAbility.enabled = false;
        if (auraAbility != null) auraAbility.enabled = false;
    }

    private void Start()
    {
        // Start with projectile unlocked at level 1
        UnlockOrUpgrade("Projectile");
    }

    private void RegisterAbility(string name, BaseAbility component)
    {
        if (component == null || string.IsNullOrEmpty(name))
        {
            return;
        }

        abilityStates[name] = new AbilityState
        {
            Name = name,
            Component = component,
            Level = 0
        };
    }

    public List<AbilityOption> GetUpgradeOptions()
    {
        var options = new List<AbilityOption>();
        foreach (var state in abilityStates.Values)
        {
            if (state.Component == null)
            {
                continue;
            }

            var stats = ConfigManager.Instance?.GetAbilityStats(state.Name);
            string description = stats?.Description;

            if (!state.Unlocked)
            {
                options.Add(new AbilityOption
                {
                    Name = state.Name,
                    Description = description,
                    CurrentLevel = 0,
                    NextLevel = 1,
                    IsNew = true
                });
            }
            else if (state.Level < maxLevelPerAbility)
            {
                options.Add(new AbilityOption
                {
                    Name = state.Name,
                    Description = description,
                    CurrentLevel = state.Level,
                    NextLevel = state.Level + 1,
                    IsNew = false
                });
            }
        }

        return options;
    }

    public void ApplyUpgrade(string abilityName)
    {
        UnlockOrUpgrade(abilityName);
    }

    private void UnlockOrUpgrade(string abilityName)
    {
        if (!abilityStates.TryGetValue(abilityName, out var state) || state.Component == null)
        {
            Debug.LogWarning($"Ability {abilityName} not registered.");
            return;
        }

        state.Level = Mathf.Clamp(state.Level + 1, 1, maxLevelPerAbility);
        var stats = ConfigManager.Instance?.GetAbilityStats(abilityName);
        state.Component.Configure(stats, state.Level);
        state.Component.enabled = true;
    }
}
