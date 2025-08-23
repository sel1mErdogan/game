using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Düşman Ayarları")]
    [Tooltip("Doğacak düşmanların prefab'ları.")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Tooltip("Düşmanların doğacağı noktalar.")]
    [SerializeField] private Transform[] spawnPoints;

    [Tooltip("Her gece kaç düşman doğacağı.")]
    [SerializeField] private int enemiesPerWave = 3;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    // Bu fonksiyonu, gece başladığında D.cs'den çağıracağız.
    public void SpawnWave()
    {
        // Önceki geceden kalan düşman varsa, temizle.
        CleanupEnemies();
        
        Debug.Log("GECE BAŞLADI! Düşmanlar beliriyor...");

        for (int i = 0; i < enemiesPerWave; i++)
        {
            if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
            {
                Debug.LogError("Enemy Spawner'a düşman prefabı veya spawn noktası atanmamış!");
                return;
            }
            
            GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
            spawnedEnemies.Add(spawnedEnemy);
        }
    }
    
    // Bu fonksiyonu, gündüz başladığında D.cs'den çağırarak hayatta kalan düşmanları silebiliriz.
    public void CleanupEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] != null)
            {
                Destroy(spawnedEnemies[i]);
            }
        }
        spawnedEnemies.Clear();
    }
}