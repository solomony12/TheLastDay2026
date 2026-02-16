using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform target;

    Animator animator;

    private float chaseRadius = 8.0f;
    private float viewAngle = 60f;

    private readonly float agentSpeed = 1.5f;

    private float attackRadius = 2.0f;
    private readonly float attackCooldown = 2.0f;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    [SerializeField] private float attackDuration = 3.0f;

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
        AttackTarget();
    }

    private void InitializeAgentInfo()
    {
        agent.speed = agentSpeed;
        agent.acceleration = 1.0f;
        agent.stoppingDistance = attackRadius;

        idleCenter = transform.position;

        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", false);
    }

    private void MoveToTarget()
    {
        if (zombieCured) return;

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        bool inChaseRange = distance <= chaseRadius;
        bool inView = false;

        if (inChaseRange)
        {
            // Check line-of-sight and field-of-view
            Ray ray = new Ray(transform.position + Vector3.up, direction.normalized);
            float angleToTarget = Vector3.Angle(transform.forward, direction);

            if (Physics.Raycast(ray, out RaycastHit hit, chaseRadius))
            {
                if (hit.transform.root == target && angleToTarget <= viewAngle / 2)
                {
                    inView = true;
                }
            }
        }

        // --- CHASE PLAYER ---
        if ((inView || targetSpotted) && inChaseRange)
        {
            StopIdle(); // stop idling
            targetSpotted = true;
            goingToLastKnownPosition = false;
            waitingAtLastKnownPosition = false;

            agent.isStopped = false;
            agent.speed = agentSpeed;
            lastKnownTargetPosition = target.position;
            agent.destination = lastKnownTargetPosition;
            animator.SetBool("isWalking", true);

            followTimer = followWaitCooldown;
            return;
        }

        // Player completely out of chase range -> lose target
        if (!inChaseRange)
        {
            targetSpotted = false;
        }

        // --- MOVE TO LAST KNOWN POSITION ---
        if (goingToLastKnownPosition)
        {
            agent.isStopped = false;
            agent.destination = lastKnownTargetPosition;
            animator.SetBool("isWalking", true);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                goingToLastKnownPosition = false;
                waitingAtLastKnownPosition = true;
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
            }
            return;
        }

        // --- WAIT AT LAST KNOWN POSITION ---
        if (waitingAtLastKnownPosition)
        {
            followTimer -= Time.deltaTime;
            if (followTimer <= 0f)
            {
                waitingAtLastKnownPosition = false;
                followTimer = followWaitCooldown;
                StartIdle(); // start idling after waiting
            }
            return;
        }

        // --- IDLE IF NOTHING ELSE ---
        if (!isIdling && idleCoroutine == null)
        {
            StartIdle();
        }
    }

    private void StartIdle()
    {
        idleCoroutine = StartCoroutine(IdleMovement());
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
            attackTimer -= Time.deltaTime;

        if (isAttacking)
            return;

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (targetSpotted && distance <= attackRadius && attackTimer <= 0f)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        Debug.Log("Attack");
        isAttacking = true;
        attackTimer = attackCooldown;

        // Stop movement
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Face target while attacking
        Vector3 lookDir = (target.position - transform.position).normalized;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        // Apply damage
        CureSystem.Instance.InfectPlayer();
        // TODO: Attack animation
        animator.SetBool("isWalking", false);

        // Wait for attack animation duration
        yield return new WaitForSeconds(attackDuration);

        animator.SetBool("isWalking", true);

        // Resume movement
        agent.isStopped = false;
        isAttacking = false;
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
                    animator.SetBool("isWalking", true);
                }
            }

            // Wait until zombie reaches the destination
            if (isIdling && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Pause and look around
                agent.isStopped = true;
                yield return new WaitForSeconds(lookAroundTime);
                isIdling = false;
                animator.SetBool("isWalking", false);

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
        animator.speed = 0f;
    }

    public void ResumeZombie()
    {
        agent.isStopped = false;

        agent.velocity = storedVelocity;
        agent.acceleration = storedAcceleration;
        animator.speed = 1f;
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

        // Draw attack line
        Vector3 attackLine = transform.position + transform.forward * attackRadius;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, attackLine);

        // Draw left and right field-of-view lines
        float halfAngle = viewAngle / 2;
        Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * chaseRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * chaseRadius);
    }
}
