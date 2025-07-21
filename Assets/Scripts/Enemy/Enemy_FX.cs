using UnityEngine;

public class Enemy_FX : MonoBehaviour
{
    [SerializeField] private Transform hitFxPosition;
    private CharacterStats stats;

    private void Awake()
    {
        stats = GetComponent<CharacterStats>();
    }

    private void OnEnable()
    {
        if (stats != null)
            stats.OnHit += PlayHitFx;
    }

    private void OnDisable()
    {
        if (stats != null)
            stats.OnHit -= PlayHitFx;
    }

    private void PlayHitFx()
    {
        if (hitFxPosition == null || HitFxManager.instance == null)
            return;

        HitFxManager.instance.PlayHitFx(hitFxPosition.position, hitFxPosition.rotation);
    }
}
