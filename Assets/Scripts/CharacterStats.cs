using System;
using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public static Action OnHealthChanged;

    public Stat_ResourceGroup resources;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;
    public Stat_MajorGroup major;

    public int level = 1; // Character level

    public float baseAttackCooldown = 1.5f; // Base attack cooldown in seconds

    public float attackCooldown => Mathf.Max(0.2f, baseAttackCooldown - offense.attackSpeed.GetValue() * 0.1f);

    [HideInInspector] public bool isDead = false;

    public float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = GetMaxHealth();
    }

    protected virtual void Start()
    {

    }
    public virtual void TakeDamage(float _damage)
    {
        if (isDead) return;

        if( AttackEvaded() )
        {
            Debug.Log($"{gameObject.name} evaded the attack!");
            return;
        }

        currentHealth -= _damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        OnHealthChanged?.Invoke();
    }

    private bool AttackEvaded()
    {
        return UnityEngine.Random.Range(0f, 100f) < GetEvasion();
    }

    public float GetDamage(out bool isCrit, float scaleFactor = 1)
    {
        float baseDamage = GetBaseDamage();
        float critChance = GetCritChance();
        float critPower = GetCritPower() / 100; // Total crit power as multiplier ( e.g 150 / 100 = 1.5f - multiplier)

        isCrit = UnityEngine.Random.Range(0, 100) < critChance;
        float finalDamage = isCrit ? baseDamage * critPower : baseDamage;

        return finalDamage * scaleFactor;
    }

    // Bonus damage from Strength: +1 per STR
    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue();
    //  Bonus crit chance from Agility: +0.3% per AGI 
    public float GetCritChance() => offense.critChance.GetValue() + (major.agility.GetValue() * .3f);
    // Bonus crit chance from Strength: +0.5% per STR 
    public float GetCritPower() => offense.critPower.GetValue() + (major.strength.GetValue() * .5f);


    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();

        float reductionMutliplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMutliplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100);
        float mitigationCap = .85f; // Max mitigation will be capped at 85%

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalMitigation;
    }
    // Bonus armor from Vitality: +1 per VIT 
    public float GetBaseArmor() => defense.armor.GetValue() + major.vitality.GetValue();

    public float GetArmorReduction()
    {
        // Total armor reduction as multiplier ( e.g 30 / 100 = 0.3f - multiplier)
        float finalReduction = offense.armorReduction.GetValue() / 100;

        return finalReduction;
    }

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * .5f; // Bonus evasion from Agility: +0.5% per AGI 

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 85f; // Max evasion will be capped at 85%

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);

        return finalEvasion;
    }

    public float GetMaxHealth()
    {
        float baseMaxHealth = resources.maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5;
        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;

        return finalMaxHealth;
    }

    protected virtual void Die()
    {
        if (CompareTag("Enemy"))
            GameManager.Instance?.UnregisterEnemy(GetComponent<Enemy>());

        isDead = true;
        Debug.Log($"{gameObject.name} morreu.");
        currentHealth = Mathf.Min(currentHealth, GetMaxHealth());
    }
    public virtual void UpdateHealth()
    {
        StartCoroutine(UpdateHealthWithDelay());
    }

    private IEnumerator UpdateHealthWithDelay()
    {
        yield return new WaitForSeconds(0.001f);
        currentHealth = Mathf.Min(currentHealth, GetMaxHealth());
        OnHealthChanged?.Invoke();
    }


    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;

            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Vitality: return major.vitality;

            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritPower: return offense.critPower;
            case StatType.ArmorReduction: return offense.armorReduction;

            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage: return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;

            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;

            case StatType.IceResistance: return defense.iceRes;
            case StatType.FireResistance: return defense.fireRes;
            case StatType.LightningResistance: return defense.lightningRes;

            default:
                Debug.LogWarning($"StatType {type} not implemented yet.");
                return null;
        }
    }


}
