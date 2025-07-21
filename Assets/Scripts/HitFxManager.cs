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

    public void PlayHitFx(Vector3 position, Quaternion rotation)
    {
        if (Pooler == null) return;

        GameObject fx = Pooler.GetInstanceFromPool();
        fx.transform.SetPositionAndRotation(position, rotation);
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
