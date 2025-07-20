using UnityEngine;

public enum SkillType { Passive, Active }
public enum SkillRarity { Common, Uncommon, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite icon;
    public SkillType type;
    public SkillRarity rarity;

    public int attackBonus;
    public int defenseBonus;
    public int speedBonus;
    public int healthBonus;

    public int cost;

    public int maxLevel = 5;

    // Active skill specific properties
    public GameObject activeSkillPrefab;
    public float activeCooldown = 1f;
    public float activeDuration = 0.5f;
    public float activeDamageMultiplier = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        cost = rarity switch
        {
            SkillRarity.Common => 1,
            SkillRarity.Uncommon => 2,
            SkillRarity.Rare => 3,
            SkillRarity.Epic => 4,
            SkillRarity.Legendary => 5,
            _ => cost
        };
    }
#endif
}