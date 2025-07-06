using System;
using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public static Action OnHealthChanged;

    public float baseMaxHealth = 100f;
    public float baseAttackDamage = 10f;
    public float baseAttackCooldown = 1.5f;

    // Propriedades que podem ser sobrescritas
    public virtual float BonusAttack => 0;
    public virtual float BonusHealth => 0;
    public virtual float BonusSpeed => 0;

    [HideInInspector] public bool isDead = false;

    public float maxHealth => baseMaxHealth + BonusHealth;
    public float attackDamage => baseAttackDamage + BonusAttack;
    public float attackCooldown => Mathf.Max(0.2f, baseAttackCooldown - BonusSpeed * 0.1f);

    public float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        OnHealthChanged?.Invoke();
    }

    protected virtual void Die()
    {
        if (CompareTag("Enemy"))
            GameManager.Instance?.UnregisterEnemy(GetComponent<Enemy>());

        isDead = true;
        Debug.Log($"{gameObject.name} morreu.");
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
    public virtual void UpdateHealth()
    {
        StartCoroutine(UpdateHealthWithDelay());
    }

    private IEnumerator UpdateHealthWithDelay()
    {
        yield return new WaitForSeconds(0.001f);
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke();
    }
}
