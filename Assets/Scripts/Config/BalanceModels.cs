using System;

[Serializable]
public class PlayerStats
{
    public float MoveSpeed;
    public float MaxHealth;
}

[Serializable]
public class EnemyStats
{
    public string Type;
    public float HP;
    public float Speed;
    public float Damage;
    public int XP_Value;
    public float AttackRange;
}

[Serializable]
public class AbilityStats
{
    public string Name;
    public float Damage;
    public float Cooldown;
    public float Area;
    public float Speed;
    public int ProjectileCount;
    public string Description;
}

[Serializable]
public class BalanceConfig
{
    public PlayerStats PlayerStats;
    public EnemyStats[] EnemyStats;
    public AbilityStats[] AbilityStats;
}
