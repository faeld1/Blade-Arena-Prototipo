using UnityEngine;

public class DeadState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true; // Stop the agent when entering the dead state
        enemy.agent.velocity = Vector3.zero; // Ensure the agent's velocity is zero

    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
    }


}
