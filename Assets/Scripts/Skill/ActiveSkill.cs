using System.Collections;
using UnityEngine;

public abstract class ActiveSkill : MonoBehaviour
{
    [SerializeField] protected SkillData skillData;
    [SerializeField] protected float cooldown = 1f;
    [SerializeField] protected float duration = 0.5f;
    [SerializeField] protected float damageMultiplier = 1f;

    protected Player owner;
    protected bool isActive;
    private float nextReadyTime;

    public SkillData Data => skillData;

    public void Configure(SkillData data)
    {
        skillData = data;
        if (skillData != null && skillData.type == SkillType.Active)
        {
            cooldown = skillData.activeCooldown;
            duration = skillData.activeDuration;
            damageMultiplier = skillData.activeDamageMultiplier;
        }
    }

    protected virtual void Awake()
    {
        owner = GetComponentInParent<Player>();
        if (skillData != null && skillData.type == SkillType.Active)
        {
            cooldown = skillData.activeCooldown;
            duration = skillData.activeDuration;
            damageMultiplier = skillData.activeDamageMultiplier;
        }
        gameObject.SetActive(false);
    }

    public bool IsOnCooldown => Time.time < nextReadyTime;

    public void TryUse()
    {
        if (!IsOnCooldown)
            StartCoroutine(UseCoroutine());
    }

    private IEnumerator UseCoroutine()
    {
        isActive = true;
        gameObject.SetActive(true);
        OnActivate();
        nextReadyTime = Time.time + cooldown;
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        isActive = false;
        OnDeactivate();
    }

    protected virtual void OnActivate() {}
    protected virtual void OnDeactivate() {}

    protected float CalculateDamage()
    {
        if (owner == null) return 0f;
        bool crit;
        return owner.Stats.GetDamage(out crit, damageMultiplier);
    }
}
