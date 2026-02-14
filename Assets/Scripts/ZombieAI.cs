using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform target;

    private float chaseRadius = 8.0f;
    public float viewAngle = 60f;

    public bool targetSpotted = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InitializeAgentInfo();
    }

    private void Update()
    {
        MoveToTarget();
    }

    private void InitializeAgentInfo()
    {
        agent.speed = 1.0f;
        agent.acceleration = 1.0f;
        agent.stoppingDistance = 0.1f;
    }

    private void MoveToTarget()
    {
        // Within distance and line-of-sight to start chasing
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        // Check distance
        if (distance <= chaseRadius)
        {
            // Check line-of-sight
            Ray ray = new Ray(transform.position + Vector3.up, direction.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, chaseRadius))
            {
                if (hit.transform == target)
                {
                    // Check if within field of view
                    float angleToTarget = Vector3.Angle(transform.forward, direction);
                    if (targetSpotted || angleToTarget <= viewAngle / 2)
                    {
                        targetSpotted = true;
                        agent.isStopped = false;
                        agent.SetDestination(target.position);
                        Debug.Log("Chase");
                        return;
                    }
                }
            }
        }

        targetSpotted = false;
        agent.isStopped = true;
    }

    public void ZombieCured()
    {
        // cured zombie doesn't attack or move
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Draw forward line
        Vector3 forwardLine = transform.position + transform.forward * chaseRadius;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, forwardLine);

        // Draw left and right field-of-view lines
        float halfAngle = viewAngle / 2;
        Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * chaseRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * chaseRadius);
    }
}
