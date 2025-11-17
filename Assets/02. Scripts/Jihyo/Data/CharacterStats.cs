using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [SerializeField] private int level = 1;
    [SerializeField] private int experience = 0;
    [SerializeField] private int maxHealth = 70;
    [SerializeField] private int healthGrowthPerLevel = 5;
    [SerializeField] private int attack = 4;
    [SerializeField] private int attackGrowthPerLevel = 0;
    [SerializeField] private int maxMagicSlots = 2;

    public int Level => level;
    public int Experience => experience;
    public int MaxHealth => maxHealth;
    public int HealthGrowthPerLevel => healthGrowthPerLevel;
    public int Attack => attack;
    public int AttackGrowthPerLevel => attackGrowthPerLevel;
    public int MaxMagicSlots => maxMagicSlots;

    public CharacterStats(int level = 1, int experience = 0, int maxHealth = 70, int healthGrowthPerLevel = 5, int attack = 4, int attackGrowthPerLevel = 0, int maxMagicSlots = 2)
    {
        this.level = level;
        this.experience = experience;
        this.maxHealth = maxHealth;
        this.healthGrowthPerLevel = healthGrowthPerLevel;
        this.attack = attack;
        this.attackGrowthPerLevel = attackGrowthPerLevel;
        this.maxMagicSlots = maxMagicSlots;
    }

    // 경험치 계산식: 25 + 6 * (level - 1)
    public static int GetRequiredExperienceForLevel(int level)
    {
        if (level < 1)
        {
            Debug.LogError("Level must be 1 or greater.");
        }

        return 25 + 6 * (level - 1);
    }
}