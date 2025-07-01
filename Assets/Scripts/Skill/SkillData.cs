using UnityEngine;

public enum SkillType { Passive }
public enum SkillRarity { Common, Rare, Epic, Legendary }

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

    public int maxLevel = 5;
}