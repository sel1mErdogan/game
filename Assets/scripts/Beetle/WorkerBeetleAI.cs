using UnityEngine;
using UnityEngine.AI;
using KingdomBug;
using InventorySystem;

[RequireComponent(typeof(NavMeshAgent), typeof(Beetle))]
public class WorkerBeetleAI : MonoBehaviour
{
    // YENİ DURUM EKLENDİ: Üsse item teslim etme durumu
    private enum State
    {
        WanderingOnRoute,
        DetouringForResource,
        ReturningToBase // <<< YENİ
    }

    [Header("Yapay Zeka Ayarları")]
    [Tooltip("Toplanacak item'ların bulunduğu katman.")]
    [SerializeField] private LayerMask resourceLayer;
    [Tooltip("Ne kadar uzağa bir güzergah hedefi belirleyebileceği.")]
    [SerializeField] private float wanderDistance = 40f;
    [Tooltip("Yol üzerindeki item'ları ne kadar mesafeden fark edeceği.")]
    [SerializeField] private float resourceScanRadius = 15f;
    [Tooltip("Ne sıklıkla etrafını tarayacağı (saniye).")]
    [SerializeField] private float scanInterval = 0.5f;

    private NavMeshAgent agent;
    private Beetle beetle;
    private Transform colonyBase;
    private State currentState;
    
    private Vector3 routeDestination;
    private Transform targetResource;
    private float lastScanTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        beetle = GetComponent<Beetle>();
        
        GameObject baseObj = GameObject.FindGameObjectWithTag("ColonyBase");
        if (baseObj != null) 
        {
            colonyBase = baseObj.transform;
        }
        else
        {
            Debug.LogError("Sahne'de 'ColonyBase' tag'ine sahip bir Üs objesi bulunamadı! Böcekler üslerini bulamıyor.", this);
        }

        SetNewRouteDestination();
        currentState = State.WanderingOnRoute;
    }

    void Update()
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
        // GÜNCELLENDİ: Envanter dolduğunda direkt üsse dönmesini sağlıyoruz.
        if (beetle.IsInventoryFull() && currentState != State.ReturningToBase)
        {
            ReturnToBase();
        }

        switch (currentState)
        {
            case State.WanderingOnRoute:
                HandleWanderingState();
                break;
            case State.DetouringForResource:
                HandleDetourState();
                break;
            // YENİ DURUM KONTROLÜ
            case State.ReturningToBase:
                HandleReturningToBaseState(); // <<< YENİ
                break;
        }
    }

    private void HandleWanderingState()
    {
        if (Time.time > lastScanTime + scanInterval)
        {
            lastScanTime = Time.time;
            Transform foundResource = FindClosestValidResource();
            if (foundResource != null)
            {
                targetResource = foundResource;
                currentState = State.DetouringForResource;
                agent.SetDestination(targetResource.position);
                return;
            }
        }

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            SetNewRouteDestination();
        }
    }

    private void HandleDetourState()
    {
        if (targetResource == null)
        {
            ResumeRoute();
            return;
        }

        if (agent.destination != targetResource.position)
        {
            agent.SetDestination(targetResource.position);
        }
    }
    
    // YENİ FONKSİYON: Üsse dönüş durumunu yönetir
    private void HandleReturningToBaseState()
    {
        // Eğer üsse yeterince yaklaştıysa...
        if (!agent.pathPending && agent.remainingDistance < 2.5f)
        {
            DepositItemsAtBase();
        }
    }

    // YENİ FONKSİYON: Item'ları üsse bırakır ve XP kazanır
    private void DepositItemsAtBase()
    {
        var items = beetle.EmptyInventory(); // Böceğin envanterini boşalt ve item'ları al
        int deliveredItemCount = items.Count;

        // Her bir item'ı ana envantere ekle
        foreach (var itemEntry in items)
        {
            InventoryManager.Instance.AddItem(itemEntry.Key, itemEntry.Value);
        }
        
        // Teslimat yaptığı için XP kazan
        if (deliveredItemCount > 0)
        {
            GetComponent<BeetleExperience>()?.AddXP(15 * deliveredItemCount);
        }
        
        // İş bitti, tekrar gezinmeye başla
        ResumeRoute();
    }

    private void ResumeRoute()
    {
        currentState = State.WanderingOnRoute;
        targetResource = null;
        SetNewRouteDestination(); // Yeni bir hedef belirle
    }

    private void SetNewRouteDestination()
    {
        Vector3 randomDirection = (transform.forward + Random.insideUnitSphere * 0.8f).normalized;
        routeDestination = transform.position + randomDirection * wanderDistance;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(routeDestination, out hit, wanderDistance, NavMesh.AllAreas))
        {
            routeDestination = hit.position;
        }
        agent.SetDestination(routeDestination);
    }

    private void ReturnToBase()
    {
        currentState = State.ReturningToBase;
        agent.SetDestination(colonyBase.position);
    }

    private Transform FindClosestValidResource()
    {
        Collider[] resources = Physics.OverlapSphere(transform.position, resourceScanRadius, resourceLayer);
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (var collider in resources)
        {
            WorldItem item = collider.GetComponent<WorldItem>();
            if(item == null) continue;

            ItemData data = item.GetItemData();
            if(data == null) continue;

            if (beetle.CanCollectItem(data))
            {
                float dSqr = (collider.transform.position - transform.position).sqrMagnitude;
                if (dSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqr;
                    bestTarget = collider.transform;
                }
            }
        }
        return bestTarget;
    }
}