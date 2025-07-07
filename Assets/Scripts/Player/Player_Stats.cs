using System.Collections;
using UnityEngine;

public class Player_Stats : CharacterStats
{
    private Player player;

    [Header("Player Die Settings")]
    [SerializeField] private float sinkDelay = 2f;
    [SerializeField] private float sinkDistance = 2f;
    [SerializeField] private float sinkDuration = 1.5f;

    [SerializeField] private int currentXP;
    [SerializeField] private int xpToNextLevel;
    private float xpGrowthFactor = 1.25f;

    public int CurrentXP => currentXP;
    public int XpToNextLevel => xpToNextLevel;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    override protected void Start()
    {
        LoadFragmentData();
        base.Start();

        xpToNextLevel = Mathf.RoundToInt(10 * Mathf.Pow(xpGrowthFactor, level - 1)); // Initial XP to next level based on level
    }

    public void ApplySkillModifier(SkillInstance skill)
    {
        string source = skill.data.skillName;

        if (skill.data.attackBonus != 0)
            offense.damage.AddModifier(skill.data.attackBonus * skill.level, source);

        if (skill.data.healthBonus != 0)
        {
            resources.maxHealth.AddModifier(skill.data.healthBonus * skill.level, source);
            UpdateHealth();
        }

        if (skill.data.speedBonus != 0)
            offense.attackSpeed.AddModifier(skill.data.speedBonus * skill.level, source);

        if (skill.data.defenseBonus != 0)
            defense.armor.AddModifier(skill.data.defenseBonus * skill.level, source);

        UpdateAttackAnimationSpeed();
    }

    public void RemoveSkillModifier(SkillInstance skill)
    {
        string source = skill.data.skillName;

        offense.damage.RemoveModifier(source);
        resources.maxHealth.RemoveModifier(source);
        offense.attackSpeed.RemoveModifier(source);
        defense.armor.RemoveModifier(source);

        UpdateAttackAnimationSpeed();
    }

    private void UpdateAttackAnimationSpeed()
    {
        player.animator.SetFloat("AttackSpeed", 1f + offense.attackSpeed.GetValue() * 0.2f);
    }

    override protected void Die()
    {
        base.Die();
        player.animator.SetTrigger("Death");
        GameManager.Instance?.PlayerDied();
        StartCoroutine(SinkAfterDelay());
    }


    private IEnumerator SinkAfterDelay()
    {
        yield return new WaitForSeconds(sinkDelay);
        Vector3 start = transform.position;
        Vector3 end = start - new Vector3(0f, sinkDistance, 0f);
        float elapsed = 0f;
        while (elapsed < sinkDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / sinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false); // desativa o jogador após afundar
        //transform.position = end;
    }

    public void AddXP(int amount)
    {
        int xpAmount = amount * 2; // Double XP

        currentXP += xpAmount;

        CheckLevelUp();
        SaveFragmentData();
    }

    // Método para verificar se é hora de subir de nível
    private void CheckLevelUp()
    {
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    // Método para subir de nível
    private void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;  // Reduz o XP necessário para o próximo nível
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpGrowthFactor);  // Aumenta o XP necessário com base no fator de crescimento
        Debug.Log("Level Up! Novo nível: " + level);
    }

    private void SaveFragmentData()
    {
        ES3.Save("PlayerLevel", level);
        ES3.Save("PlayerCurrentXP", currentXP);
    }

    private void LoadFragmentData()
    {
        level = ES3.Load("PlayerLevel", defaultValue: 1);
        currentXP = ES3.Load("PlayerCurrentXP", defaultValue: 0);
    }
}
