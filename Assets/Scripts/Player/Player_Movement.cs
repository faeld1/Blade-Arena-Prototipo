using UnityEngine;
using Pathfinding;

public class Player_Movement : MonoBehaviour
{
    private RichAI agent;
    private Transform currentTarget;
    private Player player;
    private Transform lookTarget;
    private bool facingTarget;

    private void Start()
    {
        player = GetComponent<Player>();
        agent = GetComponent<RichAI>();

        SetTarget(player.CurrentEnemyTarget?.transform); // Set the target to the current enemy target
    }

    private void Update()
    {
        if (currentTarget != null && !agent.isStopped && GameManager.Instance.battleOngoing == true)
        {
            agent.destination = currentTarget.position;
        }

        if (facingTarget && lookTarget != null)
        {
            //transform.rotation = player.FaceTarget(lookTarget.position);
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
        if (currentTarget != null)
        {
            agent.destination = currentTarget.position;
            agent.SearchPath();
        }
    }

    public void StopMovement()
    {
        agent.isStopped = true;
    }

    public void ResumeMovement()
    {
        agent.isStopped = false;
        if (currentTarget != null)
        {
            agent.SearchPath();
        }
    }

    public bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }

    public void FaceTarget(Transform target)
    {
        lookTarget = target;
        facingTarget = target != null;
    }

    public void StopFacingTarget()
    {
        lookTarget = null;
        facingTarget = false;
    }

}
