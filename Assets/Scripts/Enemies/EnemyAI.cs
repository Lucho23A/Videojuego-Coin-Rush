using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// IA base de enemigo terrestre con FSM (Idle, Patrol, Chase, Attack).
/// Usa NavMeshAgent.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Attack, Dead }

    [Header("Configuración")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float loseSightRange = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Patrullaje")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;

    private NavMeshAgent agent;
    private Transform player;
    private State currentState = State.Idle;
    private int patrolIndex = 0;
    private float waitTimer = 0f;
    private float attackTimer = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        ChangeState(patrolPoints.Length > 0 ? State.Patrol : State.Idle);
    }

    private void Update()
    {
        if (currentState == State.Dead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
            case State.Patrol:
                if (distanceToPlayer <= detectionRange) ChangeState(State.Chase);
                else if (currentState == State.Patrol) UpdatePatrol();
                break;

            case State.Chase:
                if (distanceToPlayer > loseSightRange)
                    ChangeState(patrolPoints.Length > 0 ? State.Patrol : State.Idle);
                else if (distanceToPlayer <= attackRange) ChangeState(State.Attack);
                else agent.SetDestination(player.position);
                break;

            case State.Attack:
                if (distanceToPlayer > attackRange) ChangeState(State.Chase);
                else UpdateAttack();
                break;
        }
    }

    private void UpdatePatrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                waitTimer = 0f;
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[patrolIndex].position);
            }
        }
    }

    private void UpdateAttack()
    {
        agent.SetDestination(transform.position); // se detiene
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.TakeDamage(damage);
            Debug.Log("[Enemy] ¡Ataque!");
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
        if (newState == State.Patrol && patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    public void Die()
    {
        ChangeState(State.Dead);
        agent.isStopped = true;
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualización en editor de los rangos de detección y ataque
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
