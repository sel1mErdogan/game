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