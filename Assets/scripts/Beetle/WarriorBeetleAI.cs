using UnityEngine;
using UnityEngine.AI;
using KingdomBug;

[RequireComponent(typeof(NavMeshAgent), typeof(Beetle))]
public class WarriorBeetleAI : MonoBehaviour
{
    private enum State { Patrolling, MovingToEnemy, AttackingEnemy, ReturningToBase } // YENİ DURUM: ReturningToBase

    [Header("Genel Savaş Ayarları")]
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Davranış Modları")]
    [Tooltip("GÜNDÜZ (Savunma): Üs çevresinde bu mesafede devriye atar.")]
    [SerializeField] private float defensivePatrolRadius = 15f;
    [Tooltip("GECE (Saldırı): Üsten uzaklaşarak bu geniş mesafede düşman arar.")]
    [SerializeField] private float aggressiveSearchRadius = 40f;

    private NavMeshAgent agent;
    private Transform targetEnemy;
    private State currentState;
    private float lastAttackTime;
    private Transform colonyBase; // Üssün pozisyonunu tutacak

    private bool isAggressiveMode = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Üssü bul ve pozisyonunu kaydet
        GameObject baseObj = GameObject.FindGameObjectWithTag("ColonyBase");
        if (baseObj != null) colonyBase = baseObj.transform;
        
        // // Doğduğu an günün saatini kontrol et (D.cs'de IsNight() metodu gerektirir)
        // D dayNightSystem = FindObjectOfType<D>();
        // if (dayNightSystem != null && dayNightSystem.IsNight())
        // {
        //     SetAggressionMode(true); // Gece doğduysa direkt saldırgan başla
        // }
        // else
        // {
        //     SetAggressionMode(false); // Gündüz doğduysa savunmada başla
        // }
        // Şimdilik varsayılan olarak savunmada başlatıyoruz.
        SetAggressionMode(false);
    }

    void Update()
    {
        // ... Düşme ve kaybolma kontrolleri eklenebilir ...
        
        switch (currentState)
        {
            case State.Patrolling:       HandlePatrollingState();    break;
            case State.MovingToEnemy:    HandleMovingToEnemyState(); break;
            case State.AttackingEnemy:   HandleAttackingEnemyState();break;
            case State.ReturningToBase:  HandleReturningState();     break;
        }
    }
    
    // --- GameManager'ın Aradığı Fonksiyon Bu ---
    public void SetAggressionMode(bool isAggressive)
    {
        isAggressiveMode = isAggressive;
        Debug.Log($"{gameObject.name} şimdi {(isAggressive ? "SALDIRI" : "SAVUNMA")} moduna geçti.");
        targetEnemy = null; // Eski hedefi unut

        if (isAggressive)
        {
            ChangeState(State.Patrolling); // Gece oldu, devriyeye çık
        }
        else
        {
            ChangeState(State.ReturningToBase); // Gündüz oldu, üsse dön
        }
    }
    // -----------------------------------------

    private void HandleReturningState()
    {
        if (colonyBase == null) return;
        
        agent.SetDestination(colonyBase.position);
        // Üsse yeterince yaklaştıysa, savunma devriyesine başla
        if (!agent.pathPending && agent.remainingDistance < 2f)
        {
            ChangeState(State.Patrolling);
        }
    }

    private void HandlePatrollingState()
    {
        // Gündüzse ve üsten çok uzaktaysa, önce üsse dön
        if (!isAggressiveMode && colonyBase != null && Vector3.Distance(transform.position, colonyBase.position) > defensivePatrolRadius + 2f)
        {
            ChangeState(State.ReturningToBase);
            return;
        }

        // Moduna göre düşman ara
        float currentSearchRadius = isAggressiveMode ? aggressiveSearchRadius : defensivePatrolRadius;
        SearchForEnemy(currentSearchRadius);

        // Hedefi yoksa ve devriye noktasina ulaştıysa, yeni nokta belirle
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            Wander();
        }
    }

    private void Wander()
    {
        Vector3 patrolCenter = isAggressiveMode ? transform.position : colonyBase.position;
        float patrolRadius = isAggressiveMode ? 20f : defensivePatrolRadius;

        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += patrolCenter;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
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
    
    private void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        agent.isStopped = false;
    }

    public void IncreaseDamage(int amount)
    {
        attackDamage += amount;
    }

    public void DecreaseAttackCooldown(float amount)
    {
        attackCooldown -= amount;
        if (attackCooldown < 0.1f) attackCooldown = 0.1f;
    }
}