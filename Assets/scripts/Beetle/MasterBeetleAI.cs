using UnityEngine;
using UnityEngine.AI;
using System.Linq; // En yakını bulmak için bu gerekli

[RequireComponent(typeof(NavMeshAgent))]
public class MasterBeetleAI : MonoBehaviour
{
    private enum State
    {
        Idle,
        MovingToBuildSite,
        Building
    }

    // --- YENİ EKLENDİ: İş Arama Ayarları ---
    [Header("İş Arama Ayarları")]
    [Tooltip("Ne kadar mesafedeki inşaat alanlarını fark edeceği.")]
    [SerializeField] private float constructionCheckRadius = 50f;
    [Tooltip("Ne sıklıkla yeni iş arayacağı (saniye).")]
    [SerializeField] private float constructionCheckInterval = 2f;
    // --- BİTTİ ---

    private NavMeshAgent agent;
    private State currentState;
    private Transform buildTarget;
    private float lastCheckTime; // Son iş arama zamanını tutar

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Idle;
    }

    private void Update()
    {
        // ... Kaybolma ve Düşme kontrolleri aynı kalabilir ...
        if (Vector3.Distance(transform.position, Vector3.zero) > 200f)
        {
            Debug.LogError($"BÖCEK KAYBOLDU! {gameObject.name} pozisyon: {transform.position}");
            transform.position = Vector3.zero + Vector3.up * 2f;
        }
        if (transform.position.y < -10f)
        {
            Debug.LogError($"BÖCEK DÜŞTÜ! {gameObject.name} Y pozisyonu: {transform.position.y}");
            transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
        }
        if (agent != null && !agent.isOnNavMesh)
        {
            Debug.LogWarning($"BÖCEK NAVMESH DIŞINDA! {gameObject.name}");
        }
        
        // --- GÜNCELLENEN MANTIK ---
        switch (currentState)
        {
            case State.Idle:
                HandleIdleState(); // Boştayken iş aramasını sağla
                break;
            case State.MovingToBuildSite:
                HandleMovingState();
                break;
            case State.Building:
                // İnşaat yaparken bir şey yapmasına gerek yok, ConstructionSite hallediyor.
                break;
        }
    }

    // --- YENİ EKLENEN FONKSİYON ---
    private void HandleIdleState()
    {
        // Belirli aralıklarla yeni iş ara
        if (Time.time > lastCheckTime + constructionCheckInterval)
        {
            lastCheckTime = Time.time;
            Transform availableSite = FindAvailableConstructionSite();
            if (availableSite != null)
            {
                AssignBuildTask(availableSite);
            }
        }
    }

    private void HandleMovingState()
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
                // İnşaat alanına "inşaatı ben yapıyorum" diye haber ver
                site.StartConstructing(this);
            }
        }
    }

    // Bu fonksiyon artık public olmak zorunda değil, ama öyle kalabilir.
    public void AssignBuildTask(Transform constructionSite)
    {
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

    // --- YENİ EKLENEN FONKSİYON: Sahipsiz inşaat alanı bulur ---
    private Transform FindAvailableConstructionSite()
    {
        // Etrafındaki tüm ConstructionSite'ları bul
        var allSites = FindObjectsOfType<ConstructionSite>();

        // Sahipsiz ve en yakın olanı bul
        ConstructionSite closestSite = allSites
            .Where(site => !site.IsAssigned() && Vector3.Distance(transform.position, site.transform.position) <= constructionCheckRadius)
            .OrderBy(site => Vector3.Distance(transform.position, site.transform.position))
            .FirstOrDefault();

        if (closestSite != null)
        {
            return closestSite.transform;
        }

        return null; // Boşta iş yok
    }
}