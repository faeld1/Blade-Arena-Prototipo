using UnityEngine;
using System.Collections.Generic;

public class SwordSlashSkill : ActiveSkill
{
    [SerializeField] private BoxCollider hitBox;
    private readonly HashSet<Enemy> enemiesHit = new();

    protected override void Awake()
    {
        base.Awake();
        if (hitBox == null)
            hitBox = GetComponent<BoxCollider>();
        if (hitBox != null)
            hitBox.isTrigger = true;
    }

    protected override void OnActivate()
    {
        enemiesHit.Clear();
        // VFX is played when the GameObject becomes active
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (!other.CompareTag("Enemy")) return;

        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && !enemy.stats.isDead && !enemiesHit.Contains(enemy))
        {
            float dmg = CalculateDamage();
            enemy.stats.TakeDamage(dmg);
            enemiesHit.Add(enemy);
        }
    }
}
