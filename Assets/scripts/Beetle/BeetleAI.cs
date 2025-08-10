// BeetleAI.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using InventorySystem; // Bu using'ler projenizde varsa kalsın
using KingdomBug;     // Bu using'ler projenizde varsa kalsın

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Beetle))]
public class BeetleAI : MonoBehaviour
{
    [Header("Genel Ayarlar")]
    [SerializeField] private float decisionInterval = 3f;
    
    [Header("Kaynak Toplama Ayarları")]
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private LayerMask resourceLayer;

    // YENİ EKLENDİ: Savaşçı Ayarları
    [Header("Savaşçı Ayarları")]
    [SerializeField] private float enemySearchRadius = 20f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f; // Saniyede bir saldırı
    [SerializeField] private LayerMask enemyLayer; // Düşmanların hangi katmanda olduğunu seçeceğiz
    
    private NavMeshAgent agent;
    private Beetle beetle;
    private BeetleType beetleType;
    private float lastDecisionTime;
    
    // YENİ EKLENDİ: Enum'a yeni durumlar ekledik
    private enum BeetleState
    {
        Idle,
        SearchingResource,
        CollectingResource,
        ReturningToBase,
        // Savaşçı için yeni durumlar
        SearchingForEnemy,
        MovingToEnemy,
        AttackingEnemy
    }
    
    private BeetleState currentState = BeetleState.Idle;
    private Transform targetResource;
    private Transform colonyBase;

    // YENİ EKLENDİ: Savaşçı için yeni değişkenler
    private Transform targetEnemy;
    private float lastAttackTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        beetle = GetComponent<Beetle>();
        beetleType = beetle.GetBeetleType();
        lastDecisionTime = Time.time;
        
        GameObject baseObj = GameObject.FindGameObjectWithTag("ColonyBase");
        if (baseObj != null)
        {
            colonyBase = baseObj.transform;
        }
        else
        {
            Debug.LogError("Koloni üssü bulunamadı! 'ColonyBase' tag'ine sahip bir obje oluşturun.");
        }
        
        PlaceOnNavMesh();
        MakeDecision();
    }
    
    private void PlaceOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogError($"{gameObject.name} için yakında NavMesh bulunamadı!");
        }
    }
    
    private void Update()
    {
        // Karar verme mekanizması, eğer boşta ise daha sık karar verebilir
        float currentDecisionInterval = (currentState == BeetleState.Idle || currentState == BeetleState.SearchingForEnemy) ? 1f : decisionInterval;
        if (Time.time - lastDecisionTime > currentDecisionInterval)
        {
            MakeDecision();
            lastDecisionTime = Time.time;
        }

        // Mevcut duruma göre sürekli güncellenen davranışlar
        // YENİ EKLENDİ: Savaşçı durumları için case'ler eklendi
        switch (currentState)
        {
            case BeetleState.Idle:
                agent.isStopped = true;
                break;
                
            case BeetleState.SearchingResource:
                // Karar verme mekanizması bu durumu tetikler, burada sürekli bir eyleme gerek yok.
                break;
                
            case BeetleState.CollectingResource:
                HandleCollectingResource();
                break;
                
            case BeetleState.ReturningToBase:
                HandleReturningToBase();
                break;

            // --- YENİ SAVAŞÇI DAVRANIŞLARI ---
            case BeetleState.SearchingForEnemy:
                // Karar verme mekanizması tetikler, sürekli eylem gerekmez.
                // İsterseniz burada rastgele gezinme (patrol) davranışı eklenebilir.
                agent.isStopped = true;
                break;

            case BeetleState.MovingToEnemy:
                HandleMovingToEnemy();
                break;

            case BeetleState.AttackingEnemy:
                HandleAttackingEnemy();
                break;
        }
    }
    
    private void MakeDecision()
    {
        // Eğer hedefimiz olan düşman ölmüşse, hedefi sıfırla
        if (targetEnemy == null && (currentState == BeetleState.MovingToEnemy || currentState == BeetleState.AttackingEnemy))
        {
            currentState = BeetleState.SearchingForEnemy;
        }

        // İşçi/Keşifçi böceklerin envanteri doluysa üsse dön
        if ((beetleType == BeetleType.Worker || beetleType == BeetleType.Explorer) && beetle.HasItems())
        {
            // Eğer zaten üsse dönmüyorsa, üsse dönmeye başla
            if (currentState != BeetleState.ReturningToBase)
            {
                Debug.Log($"{gameObject.name} ({beetleType}) envanteri dolu, üsse dönüyor.");
                currentState = BeetleState.ReturningToBase;
            }
            return;
        }
    
        // Böcek türüne göre ana davranış
        switch (beetleType)
        {
            case BeetleType.Worker:
            case BeetleType.Explorer:
                // Eğer böcek boştaysa veya bir kaynağı toplamayı bitirdiyse, yeni kaynak ara
                if (currentState == BeetleState.Idle || currentState == BeetleState.CollectingResource)
                {
                    Debug.Log($"{gameObject.name} ({beetleType}) kaynak arıyor.");
                    currentState = BeetleState.SearchingResource;
                    SearchForResource();
                }
                break;
            
            case BeetleType.Master:
                // Usta böcek mantığı
                currentState = BeetleState.Idle;
                break;
            
            case BeetleType.Warrior:
                // Savaşçı böcek mantığı
                if (targetEnemy == null)
                {
                    currentState = BeetleState.SearchingForEnemy;
                    SearchForEnemy();
                }
                break;
        }
    }


    #region Kaynak Toplama Davranışları
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


    private void HandleCollectingResource()
    {
        if (targetResource == null) { currentState = BeetleState.Idle; return; }
        agent.SetDestination(targetResource.position);
        if (Vector3.Distance(transform.position, targetResource.position) < 1.5f)
        {
            currentState = BeetleState.Idle;
        }
    }

    private void HandleReturningToBase()
    {
        if (colonyBase == null) 
        { 
            Debug.LogError("Koloni üssü bulunamadı!");
            currentState = BeetleState.Idle; 
            return; 
        }
    
        // Böceğin üsse doğru gitmesini sağla
        agent.isStopped = false;
        agent.SetDestination(colonyBase.position);
    
        // Eğer böcek üsse yeterince yaklaştıysa, durumunu değiştir
        // Burada envanteri boşaltmıyoruz çünkü ColonyBase sınıfı OnTriggerEnter ile bunu yapıyor
        if (Vector3.Distance(transform.position, colonyBase.position) < 2f)
        {
            Debug.Log($"{gameObject.name} ({beetleType}) üsse ulaştı. Envanteri boşaltılacak.");
            // Durumu boşta olarak güncelle
            currentState = BeetleState.Idle;
        
            // Böcek üsse ulaştığında yeni bir karar vermesi için zamanı sıfırla
            lastDecisionTime = Time.time - decisionInterval; // Hemen yeni karar almasını sağla
        }
    }


    #endregion

    #region Savaşçı Davranışları (YENİ EKLENEN FONKSİYONLAR)

    // Etrafta düşman arar
    private void SearchForEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, enemySearchRadius, enemyLayer);

        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy;
            currentState = BeetleState.MovingToEnemy;
            Debug.Log($"{gameObject.name} düşman buldu: {targetEnemy.name} ve ona doğru gidiyor.");
        }
    }

    // Düşmana doğru hareket eder
    private void HandleMovingToEnemy()
    {
        if (targetEnemy == null)
        {
            currentState = BeetleState.SearchingForEnemy;
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(targetEnemy.position);

        // Eğer saldırı menziline girdiyse, saldırma durumuna geç
        if (Vector3.Distance(transform.position, targetEnemy.position) <= attackRange)
        {
            currentState = BeetleState.AttackingEnemy;
        }
    }

    // Düşmana saldırır
    private void HandleAttackingEnemy()
    {
        if (targetEnemy == null)
        {
            currentState = BeetleState.SearchingForEnemy;
            agent.isStopped = true;
            return;
        }

        // Düşman menzilden çıkarsa tekrar peşine düş
        if (Vector3.Distance(transform.position, targetEnemy.position) > attackRange)
        {
            currentState = BeetleState.MovingToEnemy;
            return;
        }

        // Hareketi durdur ve düşmana dön
        agent.isStopped = true;
        transform.LookAt(targetEnemy);

        // Saldırı bekleme süresi dolduysa saldır
        if (Time.time > lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    // Saldırı işlemini gerçekleştirir
    private void PerformAttack()
    {
        Debug.Log($"{gameObject.name}, {targetEnemy.name} adlı düşmana saldırıyor!");
        // Burada saldırı animasyonunu tetikleyebilirsiniz.
        // Örn: animator.SetTrigger("Attack");

        // Düşmanın can sistemine erişip hasar ver
        // ÖNEMLİ: Düşman objelerinde "CanSistemi" adında bir script olmalı.
        // Bu scriptin içinde de "HasarAl(int miktar)" adında public bir fonksiyon olmalı.
        CanSistemi enemyHealth = targetEnemy.GetComponent<CanSistemi>();
        if (enemyHealth != null)
        {
            enemyHealth.HasarAl(attackDamage);
        }
        else
        {
            Debug.LogWarning($"{targetEnemy.name} üzerinde CanSistemi scripti bulunamadı!");
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        // Kaynak arama yarıçapını göster
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);

        // YENİ EKLENDİ: Düşman arama ve saldırı menzilini göster
        if (beetleType == BeetleType.Warrior)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, enemySearchRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}