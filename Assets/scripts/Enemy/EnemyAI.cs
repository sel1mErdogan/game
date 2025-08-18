using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(CanSistemi))]
public class EnemyAI : MonoBehaviour
{
    private enum State { MovingToObjective, AttackingTarget }

    [Header("Genel Ayarlar")]
    private Transform objectiveTarget;
    [Tooltip("Saldırılabilecek hedeflerin bulunduğu katman (Böcekler ve Binalar)")]
    [SerializeField] private LayerMask targetLayer;

    [Header("Saldırı Ayarları")]
    [SerializeField] private float aggroRadius = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1.5f;

    private NavMeshAgent agent;
    private State currentState;
    private Transform currentAttackTarget;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.MovingToObjective;

        GameObject colonyBaseObj = GameObject.FindGameObjectWithTag("ColonyBase");
        if (colonyBaseObj != null)
        {
            objectiveTarget = colonyBaseObj.transform;
            agent.SetDestination(objectiveTarget.position);
        }
        else
        {
            Debug.LogError("Düşman, 'ColonyBase' tag'ine sahip bir hedef bulamadı!", this);
        }
    }

    void Update()
    {
        if (currentState == State.MovingToObjective)
        {
            LookForTargets();
        }
        else if (currentState == State.AttackingTarget)
        {
            HandleAttackState();
        }
    }

    private void LookForTargets()
    {
        Collider[] potentialTargets = Physics.OverlapSphere(transform.position, aggroRadius, targetLayer);
        if (potentialTargets.Length > 0)
        {
            currentAttackTarget = potentialTargets[0].transform;
            currentState = State.AttackingTarget;
        }
    }

    private void HandleAttackState()
    {
        if (currentAttackTarget == null)
        {
            currentState = State.MovingToObjective;
            agent.isStopped = false;
            if(objectiveTarget != null) agent.SetDestination(objectiveTarget.position);
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentAttackTarget.position);

        if (distanceToTarget <= attackRange)
        {
            agent.isStopped = true;
            transform.LookAt(currentAttackTarget);

            if (Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(currentAttackTarget.position);
        }
    }

    private void Attack()
    {
        CanSistemi targetHealth = currentAttackTarget.GetComponent<CanSistemi>();
        if (targetHealth != null)
        {
            targetHealth.HasarAl(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }
}