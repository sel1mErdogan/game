using UnityEngine;
using UnityEngine.AI;
using KingdomBug;

[RequireComponent(typeof(NavMeshAgent), typeof(Beetle))]
public class WarriorBeetleAI : MonoBehaviour
{
    private enum State { Patrolling, MovingToEnemy, AttackingEnemy }

    [Header("Savaşçı Ayarları")]
    [SerializeField] private float enemySearchRadius = 15f;
    [SerializeField] private float patrolRadius = 20f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    private NavMeshAgent agent;
    private Transform targetEnemy;
    private State currentState;
    private float lastAttackTime;
    private Vector3 startPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        ChangeState(State.Patrolling);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling: HandlePatrollingState(); break;
            case State.MovingToEnemy: HandleMovingToEnemyState(); break;
            case State.AttackingEnemy: HandleAttackingEnemyState(); break;
        }
    }

    private void HandlePatrollingState()
    {
        SearchForEnemy();
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Wander();
        }
    }

    private void HandleMovingToEnemyState()
    {
        if (targetEnemy == null)
        {
            ChangeState(State.Patrolling);
            return;
        }
        agent.SetDestination(targetEnemy.position);
        if (Vector3.Distance(transform.position, targetEnemy.position) <= attackRange)
        {
            ChangeState(State.AttackingEnemy);
        }
    }

    private void HandleAttackingEnemyState()
    {
        if (targetEnemy == null)
        {
            ChangeState(State.Patrolling);
            return;
        }
        if (Vector3.Distance(transform.position, targetEnemy.position) > attackRange)
        {
            ChangeState(State.MovingToEnemy);
            return;
        }

        agent.isStopped = true;
        transform.LookAt(targetEnemy);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void SearchForEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, enemySearchRadius, enemyLayer);
        if (enemies.Length > 0)
        {
            targetEnemy = enemies[0].transform;
            ChangeState(State.MovingToEnemy);
        }
    }

    private void Attack()
    {
        CanSistemi enemyHealth = targetEnemy.GetComponent<CanSistemi>();
        if (enemyHealth != null)
        {
            enemyHealth.HasarAl(attackDamage);
        }
    }
    
    private void Wander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1)) {
            agent.SetDestination(hit.position);
        }
    }

    private void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        agent.isStopped = false;
    }
}