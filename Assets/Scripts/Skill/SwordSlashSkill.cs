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

        if (!other.CompareTag("Enemy"))
            return;

        var stats = other.GetComponent<CharacterStats>();
        if (stats != null && !stats.isDead)
        {
            float dmg = CalculateDamage();
            stats.TakeDamage(dmg);
        }
    }
}
