[System.Serializable]
public class SkillInstance
{
    public SkillData data;
    public int level;

    public SkillInstance(SkillData data)
    {
        this.data = data;
        this.level = 1;
    }

    public bool IsMaxLevel => level >= 5;
}
