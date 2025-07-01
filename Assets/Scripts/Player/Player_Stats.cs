using UnityEngine;

public class Player_Stats : CharacterStats
{
    private Player player;

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
}
