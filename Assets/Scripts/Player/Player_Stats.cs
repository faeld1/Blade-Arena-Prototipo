using System.Collections;
using UnityEngine;

public class Player_Stats : CharacterStats
{
    private Player player;

    [Header("Player Die Settings")]
    [SerializeField] private float sinkDelay = 2f;
    [SerializeField] private float sinkDistance = 2f;
    [SerializeField] private float sinkDuration = 1.5f;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    private int bonusAttack;
    private int bonusHealth;
    private int bonusSpeed;

    public override float BonusAttack => bonusAttack;
    public override float BonusHealth => bonusHealth;
    public override float BonusSpeed => bonusSpeed;

    public void SetBonuses(int atk, int hp, int spd)
    {
        bool healthChanged = bonusHealth != hp;

        bonusAttack = atk;
        bonusHealth = hp;
        bonusSpeed = spd;

        player.animator.SetFloat("AttackSpeed", 1f + (BonusSpeed * 0.1f) * 2);

        if (healthChanged)
            UpdateHealth(); // atualiza vida caso maxHealth tenha mudado
    }

    override protected void Die()
    {
        base.Die();
        player.animator.SetTrigger("Death");
        GameManager.Instance?.PlayerDied();
        StartCoroutine(SinkAfterDelay());
    }


    private IEnumerator SinkAfterDelay()
    {
        yield return new WaitForSeconds(sinkDelay);
        Vector3 start = transform.position;
        Vector3 end = start - new Vector3(0f, sinkDistance, 0f);
        float elapsed = 0f;
        while (elapsed < sinkDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / sinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false); // desativa o jogador após afundar
        //transform.position = end;
    }
}
