using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform target;

    private float chaseRadius = 8.0f;
    public float viewAngle = 60f;

    public bool targetSpotted = false;

    [Header("Idle Settings")]
    public float idleRadius = 3f;
    public float idleSpeed = 1.5f;
    public float lookAroundTime = 2f;
    public float waitTimeMin = 1f;
    public float waitTimeMax = 4f;

    private Vector3 idleCenter;
    private bool isIdling = false;

    private Vector3 storedVelocity;
    private float storedAcceleration;

    Coroutine idleCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InitializeAgentInfo();
        idleCoroutine = StartCoroutine(IdleMovement());
    }

    private void Update()
    {
        if (SettingsMenuUI.SettingsIsOpen)
        {
            return;
        }

        MoveToTarget();
    }

    private void InitializeAgentInfo()
    {
        agent.speed = 1.0f;
        agent.acceleration = 1.0f;
        agent.stoppingDistance = 0.1f;

        idleCenter = transform.position;
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
                        if (idleCoroutine != null)
                        {
                            StopCoroutine(idleCoroutine);
                            idleCoroutine = null;
                        }
                        targetSpotted = true;
                        agent.isStopped = false;
                        agent.SetDestination(target.position);
                        return;
                    }
                }
            }
        }

        targetSpotted = false;

        if (idleCoroutine == null)
        {
            idleCoroutine = StartCoroutine(IdleMovement());
        }
    }

    private IEnumerator IdleMovement()
    {
        while (true)
        {
            if (!isIdling)
            {
                // Pick a random point within idleRadius
                Vector3 randomPoint = idleCenter + Random.insideUnitSphere * idleRadius;
                randomPoint.y = transform.position.y; // Keep same height

                // Check if the point is on the NavMesh
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    agent.speed = idleSpeed;
                    agent.SetDestination(hit.position);
                    agent.isStopped = false;
                    isIdling = true;
                }
            }

            // Wait until zombie reaches the destination
            if (isIdling && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Pause and look around
                agent.isStopped = true;
                yield return new WaitForSeconds(lookAroundTime);
                isIdling = false;

                // Wait a random time before next wander
                float wait = Random.Range(waitTimeMin, waitTimeMax);
                yield return new WaitForSeconds(wait);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void PauseZombie()
    {
        agent.isStopped = true;

        storedAcceleration = agent.acceleration;
        storedVelocity = agent.velocity;

        agent.velocity = Vector3.zero;
        agent.acceleration = 0;
    }

    public void ResumeZombie()
    {
        agent.isStopped = false;

        agent.velocity = storedVelocity;
        agent.acceleration = storedAcceleration;
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
