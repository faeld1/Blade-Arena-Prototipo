using UnityEngine;
using System.Collections;

public class HitFxManager : MonoBehaviour
{
    public static HitFxManager instance;
    public ObjectPooler Pooler { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Pooler = GetComponent<ObjectPooler>();
    }

    public void PlayHitFx(Vector3 position)
    {
        if (Pooler == null) return;

        //Hit Effect
        float randomEffectX = Random.Range(-0.5f, 0.5f);
        float randomEffectY = Random.Range(0f, 1f);

        Vector3 randomEffectPosition = new Vector3(randomEffectX, randomEffectY, -0.5f);

        GameObject fx = Pooler.GetInstanceFromPool();
        fx.transform.localScale = Vector3.one * 0.4f; // Set a default scale
        fx.transform.position = position + randomEffectPosition;
        fx.SetActive(true);

        var ps = fx.GetComponent<ParticleSystem>();
        float delay = 1f;
        if (ps != null)
        {
            delay = ps.main.duration;
        }
        StartCoroutine(ObjectPooler.ReturnToPoolWithDelay(fx, delay));
    }
}
