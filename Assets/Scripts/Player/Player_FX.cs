using System.Collections;
using UnityEngine;

public class Player_FX : MonoBehaviour
{
    [SerializeField] private GameObject reviveFX;

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

        HitFxManager.instance.PlayHitFx(hitFxPosition.position);
    }

    public void PlayReviveFX()
    {
        if (reviveFX == null)
            return;

        StartCoroutine(ReviveEffectRoutine());
    }

    private IEnumerator ReviveEffectRoutine()
    {
        reviveFX.SetActive(true);

        Vector3 startPos = reviveFX.transform.localPosition;
        Vector3 endPos = startPos + Vector3.down;

        yield return new WaitForSeconds(1f);

        float duration = 2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            reviveFX.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        reviveFX.transform.localPosition = endPos;
        reviveFX.SetActive(false);
        reviveFX.transform.localPosition = startPos;
    }
}
