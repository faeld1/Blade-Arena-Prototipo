using UnityEngine;

public class Enemy_Melee : Enemy
{
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public AttackState_Melee attackState { get; private set; } // Assuming you have an AttackState_Melee class
    public DeadState_Melee deadState { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        attackState = new AttackState_Melee(this, stateMachine, "Attack"); // Initialize the attack state
        deadState = new DeadState_Melee(this, stateMachine, "Dead"); // Initialize the dead state
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();

    }

    public override void Dead()
    {
        stateMachine.ChangeState(deadState); // Change to dead state when the enemy dies
    }
}
