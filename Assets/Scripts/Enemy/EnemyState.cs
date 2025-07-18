using UnityEngine;
using Pathfinding;

public class EnemyState
{
    protected Enemy enemyBase;
    protected EnemyStateMachine stateMachine;

    protected string animBoolName;
    protected float stateTimer;

    protected bool triggerCalled;

    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        enemyBase.anim.SetBool(animBoolName, true);

        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);
    }

    public void AnimationTrigger() => triggerCalled = true;

    protected Vector3 GetNextPathPoint()
    {
        return enemyBase.agent.steeringTarget;
    }
}
