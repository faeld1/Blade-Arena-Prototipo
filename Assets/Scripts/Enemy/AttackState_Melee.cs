using UnityEngine;

public class AttackState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    public AttackState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
        stateTimer = 0.5f; // Set a timer for the attack animation duration
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();

        enemy.transform.rotation = enemy.FaceTarget(GameManager.Instance.player.transform.position); // Face the player while attacking

        if (triggerCalled)
            stateMachine.ChangeState(enemy.moveState); // Change to move state after attack animation

    }
    public override void Exit()
    {
        base.Exit();
        enemy.agent.isStopped = false; // Resume agent movement after exiting attack state
    }
}
