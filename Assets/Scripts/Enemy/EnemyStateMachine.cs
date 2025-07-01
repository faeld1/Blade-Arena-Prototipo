public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; }

    public void Initialize(EnemyState initialState)
    {
        currentState = initialState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }
}
