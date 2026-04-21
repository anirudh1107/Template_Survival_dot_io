using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    [SerializeField] private int enemyCount = 50;
    [SerializeField] private float ringSpawnFrequency = 5.0f;
    [SerializeField] private float harmonicSpawnFrequency = 5.0f;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float ringRadius = 10f;
    [SerializeField] private int numberOfEnemiesInRing = 12;
    [Header("Harmonic Pattern Settings")]
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float phaseShift = 0f;
    [SerializeField] private float speed = 0.3f;
    [SerializeField] private float harmonicunitSpawnFrequency = 0.1f;


    private IObjectPool<Enemy> ringEnemyPool;
    private IObjectPool<Enemy> harmonicEnemyPool;
    private IObjectPool<Enemy> archimedeanSpiralPool;
    private List<Enemy> activeEnemies = new List<Enemy>();
    private float verticalHeight;
    private float horizontalWidth;

    private Vector3 upperScreenBound;
    private Vector3 leftScreenBound;

   


    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CreatePools();
    }

    private void CreatePools() {
        ringEnemyPool = new ObjectPool<Enemy>(
            () => CreateEnemy(EnemyPattern.ringSpawn),
            OnGetFromPool,
            OnReleaseToPool,
            OnDestroyFromPool,
            true,
            enemyCount,
            enemyCount * 2
        );

        harmonicEnemyPool = new ObjectPool<Enemy>(
            () => CreateEnemy(EnemyPattern.harmonic),
            OnGetFromPool,
            OnReleaseToPool,
            OnDestroyFromPool,
            true,
            enemyCount,
            enemyCount * 2
        );

        archimedeanSpiralPool = new ObjectPool<Enemy>(
            () => CreateEnemy(EnemyPattern.archimedeanSpiral),
            OnGetFromPool,
            OnReleaseToPool,
            OnDestroyFromPool,
            true,
            enemyCount,
            enemyCount * 2
        );
    }

    private Enemy CreateEnemy(EnemyPattern pattern)
    {
        Enemy enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        enemy.InitEnemy(ringEnemyPool, pattern);
        return enemy;
    }
    
    private void OnGetFromPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
        enemy.ResetState();
    }

    private void OnReleaseToPool(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        enemy.gameObject.transform.position = new Vector3(-50f, -50f, 0f); 
    }

    private void OnDestroyFromPool(Enemy enemy) => Destroy(enemy.gameObject);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        verticalHeight = Camera.main.orthographicSize;
        horizontalWidth = verticalHeight * Camera.main.aspect;
        
        
        InvokeRepeating(nameof(SpawnEnemiesInRing), 8f, ringSpawnFrequency);
        InvokeRepeating(nameof(SpawnEnemiesInHarmonic), 4f, harmonicSpawnFrequency);
        //InvokeRepeating(nameof(SpawnEnemiesInArchimedeanSpiral), 15f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        upperScreenBound = new Vector3(playerTransform.position.x, playerTransform.position.y + verticalHeight, 0f);
        leftScreenBound = new Vector3(playerTransform.position.x - horizontalWidth, playerTransform.position.y, 0f);


        //

        
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            Enemy e = activeEnemies[i];

            if (e == null) continue; // Safety check
            if (e.GetCurrentPattern() == EnemyPattern.ringSpawn) // Spiral Pattern Enemy towards player
            {
                // 1. Process Logic (Move towards player)
                activeEnemies[i].transform.position = Vector3.MoveTowards(activeEnemies[i].transform.position, playerTransform.position, Time.deltaTime);
            }
            else if (e.GetCurrentPattern() == EnemyPattern.harmonic) // Harmonic Pattern Enemy
            {
                // Implement harmonic movement logic here
                HarmonicUpdate(activeEnemies[i], i);
            }
            else if (e.GetCurrentPattern() == EnemyPattern.archimedeanSpiral) // Archimedean Spiral Pattern Enemy
            {
                // Implement archimedean spiral movement logic here
            }

            
            
        }
    }

    public void SpawnRing(int count, float radius)
    {
        for (int i = 0; i < count; i++)
        {
            // Calculate angle for this specific enemy
            float angle = i * (Mathf.PI * 2f / count); 
        
            // Calculate position based on the angle
            Vector3 spawnOffset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            Vector3 spawnPos = playerTransform.position + spawnOffset;

            // Pull from our optimized pool
            Enemy enemy = ringEnemyPool.Get();
            enemy.transform.position = spawnPos;
        
            // Add to our active list for the Manager-Led Update
            enemy.SetEnemyIndex(activeEnemies.Count);
            activeEnemies.Add(enemy);
        }
    }

    public void SpawnEnemiesInRing()
    {
        SpawnRing(numberOfEnemiesInRing, ringRadius);
    }

    public void RemoveEnemyAtIndex(int index)
    {
        if (index < 0 || index >= activeEnemies.Count) return;

        // 1. Return to pool
        ringEnemyPool.Release(activeEnemies[index]);

        // 2. Optimized Swap-Back Removal
        int lastIndex = activeEnemies.Count - 1;
        activeEnemies[index] = activeEnemies[lastIndex]; // Move last enemy to current gap
        activeEnemies[index].SetEnemyIndex(index); // Update the enemy's index in the list
        activeEnemies.RemoveAt(lastIndex);           // Remove the duplicate at the end
    }

    private void HarmonicUpdate(Enemy enemy, float phaseShift = 0f)
    {
        //phaseShift += Time.deltaTime; // Adjust phase shift over time for movement variation
        // Implement harmonic movement logic here
        float x = amplitude * Mathf.Sin(Time.time * frequency + phaseShift) + upperScreenBound.x;
        float y = enemy.transform.position.y - (speed * Time.deltaTime);
        enemy.transform.position = new Vector3(x, y, 0);
    }

    public void SpawnEnemiesInHarmonic()
    {
        // Implement harmonic spawning logic here
        
            
            StartCoroutine(HarmonicSpawnDelay());
        
    }

    public void SpawnEnemiesInArchimedeanSpiral()
    {
        // Implement archimedean spiral spawning logic here
    }

    public enum EnemyPattern{
        ringSpawn,
        harmonic,
        archimedeanSpiral,
        BezierCurves
    }

    private System.Collections.IEnumerator HarmonicSpawnDelay()
    {
        for (int i = 0; i < numberOfEnemiesInRing; i++)
        {
            Vector3 spawnPos = new Vector3(upperScreenBound.x, upperScreenBound.y, 0f);
            Enemy enemy = harmonicEnemyPool.Get();
            enemy.transform.position = spawnPos;
            enemy.SetEnemyIndex(activeEnemies.Count);
            activeEnemies.Add(enemy);
            yield return new WaitForSeconds(harmonicunitSpawnFrequency); 
        }
        
    }

}