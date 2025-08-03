using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using InventorySystem;
using KingdomBug;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Beetle))]
public class BeetleAI : MonoBehaviour
{
    [SerializeField] private float decisionInterval = 3f;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private LayerMask resourceLayer;
    
    private NavMeshAgent agent;
    private Beetle beetle;
    private BeetleType beetleType;
    private float lastDecisionTime;
    
    private enum BeetleState
    {
        Idle,
        SearchingResource,
        CollectingResource,
        ReturningToBase
    }
    
    private BeetleState currentState = BeetleState.Idle;
    private Transform targetResource;
    private Transform colonyBase;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        beetle = GetComponent<Beetle>();
        beetleType = beetle.GetBeetleType();
        lastDecisionTime = Time.time;
        
        // Koloni üssünü bul
        GameObject baseObj = GameObject.FindGameObjectWithTag("ColonyBase");
        if (baseObj != null)
        {
            colonyBase = baseObj.transform;
        }
        else
        {
            Debug.LogError("Koloni üssü bulunamadı! 'ColonyBase' tag'ine sahip bir obje oluşturun.");
        }
        
        // NavMesh üzerinde olduğundan emin ol
        PlaceOnNavMesh();
        
        // İlk kararı ver
        MakeDecision();
    }
    
    // Böceği NavMesh üzerine yerleştir
    private void PlaceOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.Warp(hit.position); // Ajanı doğrudan NavMesh'e yerleştir
            Debug.Log($"{gameObject.name} NavMesh üzerine yerleştirildi.");
        }
        else
        {
            Debug.LogError($"{gameObject.name} için yakında NavMesh bulunamadı!");
        }
    }
    
    private void Update()
    {
        // Belirli aralıklarla karar verme
        if (Time.time - lastDecisionTime > decisionInterval)
        {
            MakeDecision();
            lastDecisionTime = Time.time;
        }
        
        // Mevcut duruma göre davranış
        switch (currentState)
        {
            case BeetleState.Idle:
                // Boşta durma, yeni görev bekleme
                break;
                
            case BeetleState.SearchingResource:
                // Kaynak ara
                SearchForResource();
                break;
                
            case BeetleState.CollectingResource:
                // Hedef kaynağa doğru hareket et
                if (targetResource != null)
                {
                    // NavMesh üzerinde olduğundan emin ol
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(targetResource.position, out hit, 5f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }
                    
                    // Kaynağa yaklaştıysa
                    if (Vector3.Distance(transform.position, targetResource.position) < 1f)
                    {
                        // Kaynak toplama işlemi Beetle.cs'deki OnTriggerEnter tarafından yapılacak
                        // Yeni görev ara
                        currentState = BeetleState.Idle;
                    }
                }
                else
                {
                    // Hedef kaynak yok olmuşsa, yeni görev ara
                    currentState = BeetleState.Idle;
                }
                break;
                
            case BeetleState.ReturningToBase:
                // Üsse dön
                if (colonyBase != null)
                {
                    // NavMesh üzerinde olduğundan emin ol
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(colonyBase.position, out hit, 5f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }
                    
                    // Üsse yaklaştıysa
                    if (Vector3.Distance(transform.position, colonyBase.position) < 1f)
                    {
                        // Envanter boşaltma işlemi ColonyBase.cs'deki OnTriggerEnter tarafından yapılacak
                        // Yeni görev ara
                        currentState = BeetleState.Idle;
                    }
                }
                break;
        }
    }
    
    // Böcek türüne göre karar verme
    private void MakeDecision()
    {
        // Böcek envanteri doluysa üsse dön
        if (beetle.HasItems())
        {
            currentState = BeetleState.ReturningToBase;
            return;
        }
        
        // Böcek türüne göre davranış
        switch (beetleType)
        {
            case BeetleType.Worker:
                // İşçi böcek malzeme toplar
                SearchForResource();
                break;
                
            case BeetleType.Explorer:
                // Keşifçi böcek haritayı keşfeder ve kırıntı/su arar
                SearchForResource();
                break;
                
            case BeetleType.Master:
                // Usta böcek inşa edilecek yapıları arar
                // (Bu kısım daha sonra yapı inşa sistemi eklendiğinde tamamlanacak)
                break;
                
            case BeetleType.Warrior:
                // Savaşçı böcek savunma bölgelerine yönelir
                // (Bu kısım daha sonra savunma sistemi eklendiğinde tamamlanacak)
                break;
        }
    }
    
    // Kaynak arama
    private void SearchForResource()
    {
        // Çevredeki kaynakları bul
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, resourceLayer);
        Debug.Log($"{gameObject.name} ({beetleType}) çevrede {hitColliders.Length} kaynak buldu.");
    
        // Uygun kaynak var mı kontrol et
        foreach (var hitCollider in hitColliders)
        {
            WorldItem worldItem = hitCollider.GetComponent<WorldItem>();
        
            if (worldItem != null)
            {
                ItemData itemData = worldItem.GetItemData();
                Debug.Log($"{gameObject.name} ({beetleType}) {itemData.itemName} kaynağını kontrol ediyor.");
            
                if (beetle.CanCollectItem(itemData))
                {
                    // Uygun kaynak bulundu, hedef olarak belirle
                    Debug.Log($"{gameObject.name} ({beetleType}) {itemData.itemName} kaynağını toplamaya gidiyor.");
                    targetResource = hitCollider.transform;
                    currentState = BeetleState.CollectingResource;
                    return;
                }
                else
                {
                    Debug.Log($"{gameObject.name} ({beetleType}) {itemData.itemName} kaynağını TOPLAYAMAZ.");
                }
            }
        }
    
        // Uygun kaynak bulunamadı, rastgele bir noktaya git
        Debug.Log($"{gameObject.name} ({beetleType}) uygun kaynak bulamadı, rastgele dolaşıyor.");
        // ... (Geri kalan kod)
    }

    
    // Arama mesafesini görselleştirmek için (sadece editor'da)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
