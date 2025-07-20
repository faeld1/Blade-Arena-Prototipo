using UnityEngine;

public class SwordSlashSkill : ActiveSkill
{
    [SerializeField] private BoxCollider hitBox;

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
        // VFX is played when the GameObject becomes active
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemy.stats.isDead)
        {
            float dmg = CalculateDamage();
            enemy.stats.TakeDamage(dmg);
        }
    }
}
