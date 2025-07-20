using System.Collections;
using UnityEngine;

public abstract class ActiveSkill : MonoBehaviour
{
    [SerializeField] protected float cooldown = 1f;
    [SerializeField] protected float duration = 0.5f;
    [SerializeField] protected float damageMultiplier = 1f;
    [SerializeField] protected float activationRange = 1.5f;
    [SerializeField] protected string animationTrigger = "";
    [SerializeField] private SkillData data;

    [SerializeField] protected Player owner;
    protected bool isActive;
    private float nextReadyTime;

    protected virtual void Awake()
    {
        owner = GetComponentInParent<Player>();
        if (owner == null)
            owner = GetComponentInParent<Player>();
        gameObject.SetActive(false);
    }

    public SkillData Data => data;
    public float Range => activationRange;
    public string AnimationTrigger => animationTrigger;
    public bool IsOnCooldown => Time.time < nextReadyTime;
    public float CooldownRemaining => Mathf.Max(0f, nextReadyTime - Time.time);
    public float CooldownDuration => cooldown;

    public void SetOwner(Player player) => owner = player;

    public void TryUse()
    {
        if (!IsOnCooldown)
        {
            // Start the coroutine from the owner so it works even when this
            // GameObject is disabled.
            if (owner != null)
                owner.StartCoroutine(UseCoroutine());
            // Start the coroutine from an active object so it works even when
            // this GameObject is disabled.
            MonoBehaviour runner = owner != null
                ? owner as MonoBehaviour
                : SkillManager.Instance as MonoBehaviour;

            if (runner != null)
                runner.StartCoroutine(UseCoroutine());
            else
                StartCoroutine(UseCoroutine());
        }
    }

    private IEnumerator UseCoroutine()
    {
        gameObject.SetActive(true);
        isActive = true;
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
