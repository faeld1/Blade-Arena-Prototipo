using UnityEngine;
using System.Collections.Generic;

public class VerticalCleaveSkill : ActiveSkill
{
    [SerializeField] private BoxCollider hitBox;
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 originalLocalPosition;
    private readonly HashSet<Enemy> enemiesHit = new();

    protected override void Awake()
    {
        base.Awake();
        if (hitBox == null)
            hitBox = GetComponent<BoxCollider>();
        if (hitBox != null)
            hitBox.isTrigger = true;
        rb = GetComponent<Rigidbody>();
        originalLocalPosition = transform.localPosition;
    }

    protected override void OnActivate()
    {
        enemiesHit.Clear();
        transform.localPosition = originalLocalPosition;
        if (owner != null)
            transform.rotation = owner.transform.rotation;
        if (rb != null)
            rb.linearVelocity = transform.forward * moveSpeed;
    }

    protected override void OnDeactivate()
    {
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
        transform.localPosition = originalLocalPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;

        if (!other.CompareTag("Enemy"))
            return;

        var enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && !enemy.stats.isDead && !enemiesHit.Contains(enemy))
        {
            float dmg = CalculateDamage();
            enemy.stats.TakeDamage(dmg);
            enemiesHit.Add(enemy);
        }
    }

    private void Reset()
    {
        activationRange = 2.1f;
        animationTrigger = "SkillVerticalCleave";
        duration = 2f;
    }
}
