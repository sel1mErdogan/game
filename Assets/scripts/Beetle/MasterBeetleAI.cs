using UnityEngine;
using UnityEngine.AI;
using KingdomBug;

[RequireComponent(typeof(NavMeshAgent), typeof(Beetle))]
public class MasterBeetleAI : MonoBehaviour
{
    private enum State
    {
        Idle,               // Boşta, görev bekliyor
        MovingToBuildSite,  // İnşaat alanına gidiyor
        Building            // İnşaat yapıyor
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
        if (currentState == State.MovingToBuildSite)
        {
            // Hedefe ulaştıysak, inşaata başla
            if (!agent.pathPending && agent.remainingDistance < 2f)
            {
                currentState = State.Building;
                agent.isStopped = true;
                // Burada "çalışma" animasyonunu başlatabilirsin
            }
        }
    }

    // BuildingManager bu fonksiyonu çağırarak görev verir
    public void AssignBuildTask(Transform constructionSite)
    {
        buildTarget = constructionSite;
        currentState = State.MovingToBuildSite;
        agent.isStopped = false;
        agent.SetDestination(buildTarget.position);
    }

    // ConstructionSite bu fonksiyonu çağırarak görevin bittiğini söyler
    public void TaskFinished()
    {
        currentState = State.Idle;
        buildTarget = null;
        // Burada "boşta durma" animasyonuna geri dönebilirsin
    }

    public bool IsAvailable()
    {
        return currentState == State.Idle;
    }
}