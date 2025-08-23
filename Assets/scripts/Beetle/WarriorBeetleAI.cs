// --- WarriorBeetleAI.cs (Tam ve Düzeltilmiş Hali) ---
using UnityEngine;
using UnityEngine.AI;
using KingdomBug;

[RequireComponent(typeof(NavMeshAgent), typeof(Beetle))]
public class WarriorBeetleAI : MonoBehaviour
{
    private enum State { Patrolling, MovingToEnemy, AttackingEnemy }

    [Header("Genel Savaş Ayarları")]
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Davranış Modları")]
    [Tooltip("GÜNDÜZ (Savunma): Üs çevresinde bu mesafede düşman arar.")]
    [SerializeField] private float defensiveSearchRadius = 15f;
    [Tooltip("GECE (Saldırı): Üsten uzaklaşarak bu geniş mesafede düşman arar.")]
    [SerializeField] private float aggressiveSearchRadius = 40f;
    [Tooltip("Devriye atacağı alanın merkezden uzaklığı.")]
    [SerializeField] private float patrolRadius = 20f;
    
    private NavMeshAgent agent;
    private Transform targetEnemy;
    private State currentState;
    private float lastAttackTime;
    private Vector3 guardPost; // Koruyacağı merkez nokta (eski startPosition yerine)

    private bool isAggressiveMode = false; // Gece modu aktif mi?

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        guardPost = transform.position; // Başladığı noktayı merkez üs say
        ChangeState(State.Patrolling);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:      HandlePatrollingState();    break;
            case State.MovingToEnemy:   HandleMovingToEnemyState(); break;
            case State.AttackingEnemy:  HandleAttackingEnemyState();break;
        }
    }
    
    // Dışarıdan çağrılarak böceğin modunu değiştirir.
    public void SetAggressionMode(bool isAggressive)
    {
        isAggressiveMode = isAggressive;
        Debug.Log($"{gameObject.name} şimdi {(isAggressive ? "SALDIRI" : "SAVUNMA")} moduna geçti.");
        targetEnemy = null;
        ChangeState(State.Patrolling);
    }

    private void HandlePatrollingState()
    {
        // Moduna göre doğru arama mesafesini kullanarak düşman ara
        float currentSearchRadius = isAggressiveMode ? aggressiveSearchRadius : defensiveSearchRadius;
        SearchForEnemy(currentSearchRadius);

        if (!agent.pathPending && agent.remainingDistance < 1.5f)
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
    
    // GÜNCELLENDİ: Artık bir parametre alıyor.
    private void SearchForEnemy(float searchRadius)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, searchRadius, enemyLayer);
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
        randomDirection += guardPost; // guardPost (eski startPosition) etrafında gezer.
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas)) 
        {
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