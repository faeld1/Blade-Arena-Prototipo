using UnityEngine;

public class IdleState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    public IdleState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemyBase.idleTime; // Set the idle time from the enemy base
    }
    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.moveState); // Change to move state after idle time
        }
    }
    public override void Exit()
    {
        base.Exit();
    }


}
