using System.Collections;
using UnityEngine;

public abstract class ActiveSkill : MonoBehaviour
{
    [SerializeField] protected float cooldown = 1f;
    [SerializeField] protected float duration = 0.5f;
    [SerializeField]
    protected float[] damageMultipliers = new float[5] { 1f, 1f, 1f, 1f, 1f };
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
    }

    public SkillData Data => data;
    public float Range => activationRange;
    public string AnimationTrigger => animationTrigger;
    public bool IsOnCooldown => Time.time < nextReadyTime;
    public float CooldownRemaining => Mathf.Max(0f, nextReadyTime - Time.time);
    public float CooldownDuration => cooldown;
    public float[] DamageMultipliers => damageMultipliers;

    public void SetOwner(Player player) => owner = player;

    public void TryUse()
    {
        if (IsOnCooldown)
            return;

        // Ensure the skill object is active before starting the coroutine.
        // This avoids issues with coroutines on disabled objects on the first use.
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // Select a MonoBehaviour to run the coroutine. Prefer the owner, but
        // fall back to the SkillManager or this component if necessary.
        MonoBehaviour runner = owner != null
            ? owner as MonoBehaviour
            : SkillManager.Instance as MonoBehaviour;

        if (runner == null)
            runner = this;


        runner.StartCoroutine(UseCoroutine());
    }

    private IEnumerator UseCoroutine()
    {
        Debug.Log("Coroutine chamada no ActiveSkill");
        isActive = true;
        OnActivate();
        nextReadyTime = Time.time + cooldown;

        if(gameObject.activeSelf)
            Debug.Log("ActiveSkill is active");

        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        isActive = false;
        OnDeactivate();
    }

    protected virtual void OnActivate() {}
    protected virtual void OnDeactivate() {}

    protected float CalculateDamage()
    {
        if (owner == null)
            return 0f;

        int level = 1;
        if (data != null && SkillManager.Instance != null)
        {
            level = SkillManager.Instance.GetSkillLevel(data);
        }

        level = Mathf.Clamp(level, 1, damageMultipliers.Length);
        float multiplier = damageMultipliers[level - 1];

        bool crit;
        return owner.Stats.GetDamage(out crit, multiplier);
    }
}
