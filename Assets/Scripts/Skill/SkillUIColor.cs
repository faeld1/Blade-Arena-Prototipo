using UnityEngine;

public static class SkillUIColor
{
    public static Color GetColor(SkillRarity rarity)
    {
        switch (rarity)
        {
            case SkillRarity.Common:
                return Color.white;
            case SkillRarity.Uncommon:
                return Color.green;
            case SkillRarity.Rare:
                return Color.blue;
            case SkillRarity.Epic:
                return new Color(0.5f, 0f, 0.5f); // purple
            case SkillRarity.Legendary:
                return new Color(1f, 0.84f, 0f); // gold
            default:
                return Color.white;
        }
    }
}
