using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterStatusEffect
{
    [SerializeField] private string name;
    [SerializeField] private string effectId;

    public string Name => name;
    public string EffectId => effectId;

    public MonsterStatusEffect(string name, string effectId)
    {
        this.name = name;
        this.effectId = effectId;
    }
}

[System.Serializable]
public class MonsterActionDefinition
{
    [SerializeField] private string name;
    [SerializeField] private string actionId;
    [SerializeField] private int value;

    public string Name => name;
    public string ActionId => actionId;
    public int Value => value;

    public MonsterActionDefinition(string name, string actionId, int value)
    {
        this.name = name;
        this.actionId = actionId;
        this.value = value;
    }
}

[System.Serializable]
public class MonsterData
{
    [SerializeField] private string id = "41001000";
    [SerializeField] private string fileName = string.Empty;
    [SerializeField] private string localizedName = "프로토타입";
    [SerializeField] private string spritePath = string.Empty;
    [SerializeField] private int hp = 45;
    [SerializeField] private MonsterType type = MonsterType.Normal;
    [SerializeField] private MonsterBehaviorPattern behaviorPattern = MonsterBehaviorPattern.Random;
    [SerializeField] private List<MonsterStatusEffect> passiveStatusEffects = new();
    [SerializeField] private List<MonsterActionDefinition> actions = new();
    [SerializeField] private int goldReward;
    [SerializeField] private int experienceReward;

    public string Id => id;
    public string FileName => fileName;
    public string LocalizedName => localizedName;
    public string SpritePath => spritePath;
    public int HP => hp;
    public MonsterType Type => type;
    public MonsterBehaviorPattern BehaviorPattern => behaviorPattern;
    public IReadOnlyList<MonsterStatusEffect> PassiveStatusEffects => passiveStatusEffects;
    public IReadOnlyList<MonsterActionDefinition> Actions => actions;
    public int? GoldReward => goldReward > 0 ? goldReward : null;
    public int? ExperienceReward => experienceReward > 0 ? experienceReward : null;

    public MonsterData( string id = "41001000", string fileName = "", string localizedName = "프로토타입", int hp = 45, MonsterType type = MonsterType.Normal,
        MonsterBehaviorPattern behaviorPattern = MonsterBehaviorPattern.Random, IReadOnlyList<MonsterStatusEffect> passiveStatusEffects = null, IReadOnlyList<MonsterActionDefinition> actions = null,
        string spritePath = "", int? goldReward = null, int? experienceReward = null)
    {
        this.id = id;
        this.fileName = fileName;
        this.localizedName = localizedName;
        this.hp = hp;
        this.type = type;
        this.behaviorPattern = behaviorPattern;
        this.passiveStatusEffects = passiveStatusEffects != null ? new List<MonsterStatusEffect>(passiveStatusEffects) : new List<MonsterStatusEffect>();
        this.actions = actions != null ? new List<MonsterActionDefinition>(actions) : new List<MonsterActionDefinition>();
        this.spritePath = spritePath;
        this.goldReward = goldReward ?? 0;
        this.experienceReward = experienceReward ?? 0;
    }
}