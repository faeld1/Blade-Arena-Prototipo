using UnityEngine;
using UnityEngine.AI;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 destination;
    private float lastTimerUpdateDestination;

    public MoveState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        destination = enemy.GetPatrolDestination(); // Get the patrol destination from the enemy base
        enemy.agent.SetDestination(destination);
    }
    public override void Update()
    {
        base.Update();

        if (enemy.PlayerInAttackRange())
        {
            stateMachine.ChangeState(enemy.attackState); // Change to attack state if player is in range
            enemy.agent.ResetPath(); // Reset the agent's path when changing states
            enemy.agent.isStopped = true; // Stop the agent when attacking
        }

        if (CanUpdateDestination())
        {
            enemy.agent.SetDestination(destination);
            destination = enemy.GetPatrolDestination(); // Update the destination periodically
        }

        enemy.transform.rotation = enemy.FaceTarget(GetNextPathPoint()); // Face the target while moving


    }

    public override void Exit()
    {
        base.Exit();

        //enemy.agent.ResetPath(); // Reset the agent's path when reaching the destination
    }



    private bool CanUpdateDestination()
    {
        if(Time.time > lastTimerUpdateDestination + 0.25f)
        {
            lastTimerUpdateDestination = Time.time;
            return true;
        }
        return false;
    }

}
