using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform target;

    private float chaseRadius = 8.0f;
    private float viewAngle = 60f;

    private readonly float agentSpeed = 1.5f;

    private float attackRadius = 1f;
    private readonly float attackCooldown = 2.0f;
    private float attackTimer = 0f;

    private Vector3 lastKnownTargetPosition;
    private bool goingToLastKnownPosition = false;
    private readonly float followWaitCooldown = 3.0f;
    private float followTimer;
    private bool waitingAtLastKnownPosition = false;

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

    private bool zombieCured = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag(Constants.playerTag).GetComponent<Transform>();
        InitializeAgentInfo();
        followTimer = followWaitCooldown;
    }

    private void Update()
    {
        if (SettingsMenuUI.SettingsIsOpen || zombieCured)
        {
            return;
        }

        MoveToTarget();
        //AttackTarget();
    }

    private void InitializeAgentInfo()
    {
        agent.speed = agentSpeed;
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
            // Check if within field of view
            float angleToTarget = Vector3.Angle(transform.forward, direction);

            if (targetSpotted ||
                (Physics.Raycast(ray, out RaycastHit hit, chaseRadius)
                && hit.transform.root == target
                && angleToTarget <= viewAngle / 2)) // already locked on or remains in view
            {
                Debug.Log("chase");
                StopIdle();
                targetSpotted = true;
                agent.isStopped = false;
                goingToLastKnownPosition = false;
                lastKnownTargetPosition = target.position;
                agent.destination = lastKnownTargetPosition;

                followTimer = followWaitCooldown;
                return;
            }
        }

        targetSpotted = false;

        // If it just lost sight, go to last known position
        if (!goingToLastKnownPosition &&
            !waitingAtLastKnownPosition &&
            idleCoroutine == null)
        {
            goingToLastKnownPosition = true;
            agent.isStopped = false;
            agent.destination = lastKnownTargetPosition;
        }

        // Arrived at last known position
        if (goingToLastKnownPosition &&
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            goingToLastKnownPosition = false;
            waitingAtLastKnownPosition = true;
            agent.isStopped = true;
        }

        // Waiting at last known position before idling
        if (waitingAtLastKnownPosition)
        {
            followTimer -= Time.deltaTime;

            if (followTimer <= 0f)
            {
                waitingAtLastKnownPosition = false;
                followTimer = followWaitCooldown;

                if (idleCoroutine == null)
                {
                    idleCoroutine = StartCoroutine(IdleMovement());
                }
            }
        }
    }

    private void StopIdle()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }

        isIdling = false;
        agent.isStopped = false;
        agent.speed = agentSpeed; // restore chase speed
    }

    private void AttackTarget()
    {
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;
        if (targetSpotted && distance <= attackRadius && attackTimer <= 0)
        {
            CureSystem.Instance.InfectPlayer();
            attackTimer = attackCooldown;
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
        zombieCured = true;
        agent.isStopped = true;
        gameObject.tag = Constants.untaggedTag;

        Destroy(agent); // remove NavMeshAgent
        Destroy(this); // remove the ZombieAI script
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
