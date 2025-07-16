using UnityEngine;

public static class SkillUIColor
{
    public static Color GetColor(SkillRarity rarity)
    {
        switch (rarity)
        {
            case SkillRarity.Common:
                return new Color(1f, 1f, 1f); // white
            case SkillRarity.Uncommon:
                return new Color(0.5f, 1f, 0.5f); // green
            case SkillRarity.Rare:
                return new Color(0.5f, 0.5f, 1f); // blue
            case SkillRarity.Epic:
                return new Color(0.85f, 0f, 0.85f); // purple
            case SkillRarity.Legendary:
                return new Color(1f, 0.9f, 0.4f); // gold
            default:
                return Color.white;
        }
    }
}
