using UnityEngine;

public class Player_Stats : CharacterStats
{
    private int bonusAttack;
    private int bonusHealth;
    private int bonusSpeed;

    public override float BonusAttack => bonusAttack;
    public override float BonusHealth => bonusHealth;
    public override float BonusSpeed => bonusSpeed;

    public void SetBonuses(int atk, int hp, int spd)
    {
        bonusAttack = atk;
        bonusHealth = hp;
        bonusSpeed = spd;

        UpdateHealth(); // atualiza vida caso maxHealth tenha mudado
    }
}
