using System.Collections;
using UnityEngine;

public class Player_Stats : CharacterStats
{
    private Player player;

    [Header("Player Die Settings")]
    [SerializeField] private float sinkDelay = 2f;
    [SerializeField] private float sinkDistance = 2f;
    [SerializeField] private float sinkDuration = 1.5f;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    public override float BonusAttack => 0;
    public override float BonusHealth => 0;
    public override float BonusSpeed => 0;

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
}
