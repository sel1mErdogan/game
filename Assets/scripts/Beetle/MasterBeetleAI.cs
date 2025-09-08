using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MasterBeetleAI : MonoBehaviour
{
    private enum State
    {
        Idle,
        MovingToBuildSite,
        Building
    }

    private NavMeshAgent agent;
    private State currentState;
    private Transform buildTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Idle;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) > 200f)
        {
            Debug.LogError($"BÖCEK KAYBOLDU! {gameObject.name} pozisyon: {transform.position}");
            // Acil durum: Böceği merkeze getir
            transform.position = Vector3.zero + Vector3.up * 2f;
        }
    
        // Y ekseninde çok aşağı düştüyse uyar
        if (transform.position.y < -10f)
        {
            Debug.LogError($"BÖCEK DÜŞTÜ! {gameObject.name} Y pozisyonu: {transform.position.y}");
            // Acil durum: Böceği yukarı çıkar
            transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
        }
    
        // NavMeshAgent durumunu kontrol et
        if (agent != null && !agent.isOnNavMesh)
        {
            Debug.LogWarning($"BÖCEK NAVMESH DIŞINDA! {gameObject.name}");
        }
        if (currentState == State.MovingToBuildSite)
        {
            if (buildTarget == null)
            {
                TaskFinished();
                return;
            }

            if (!agent.pathPending && agent.remainingDistance < 2f)
            {
                currentState = State.Building;
                agent.isStopped = true;
                
                ConstructionSite site = buildTarget.GetComponent<ConstructionSite>();
                if (site != null)
                {
                    site.StartConstructing(this);
                }
            }
        }
    }

    public void AssignBuildTask(Transform constructionSite)
    {
        // --- TEŞHİS MESAJI BURADA ---
        Debug.Log($"4. ADIM: '{gameObject.name}' adlı böcek görev emrini ALDI! Hedef: {constructionSite.position}");

        buildTarget = constructionSite;
        currentState = State.MovingToBuildSite;
        agent.isStopped = false;
        agent.SetDestination(buildTarget.position);
    }

    public void TaskFinished()
    {
        currentState = State.Idle;
        buildTarget = null;
        agent.isStopped = true;
    }

    public bool IsAvailable()
    {
        return currentState == State.Idle;
    }
}