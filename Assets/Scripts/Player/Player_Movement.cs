using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Player_Movement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform currentTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetTarget(GameManager.Instance?.activeEnemies[0].transform); // Set the target to the player transform
    }

    private void Update()
    {
        if (currentTarget != null && !agent.isStopped && GameManager.Instance.battleOngoing == true)
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }

    public void StopMovement()
    {
        agent.isStopped = true;
    }

    public void ResumeMovement()
    {
        agent.isStopped = false;
    }

    public bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }

}
