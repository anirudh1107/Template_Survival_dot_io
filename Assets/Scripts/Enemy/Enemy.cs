using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IPoolable, IDamageable
{

    public Rigidbody2D rb;
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private GameObject collectablePrefab; // Assign in Inspector
    public float _currentHealth;
    public int enemyIndex;
    private IObjectPool<Enemy> _pool;

    private SpriteRenderer _renderer;

    

    void Awake() => rb = GetComponent<Rigidbody2D>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitEnemy(IObjectPool<Enemy> pool) {
        _pool = pool;
        _currentHealth = maxHealth;
    }

    public void ResetState()
    {
        _currentHealth = maxHealth;
        rb.linearVelocity = Vector3.zero; // Reset physics
        enemyIndex = -1;
    }

    public void SetEnemyIndex(int index)
    {
        enemyIndex = index;
    }

    public void TakeDamage(float amount, Vector3 hitDirection) {
        _currentHealth -= amount;

        if (_currentHealth <= 0) {
            Die();
        }
        else
        {
            // Visual Feedback (Flash White) 
            StartCoroutine(FlashEffect());

            if (this.TryGetComponent<Knockback>(out Knockback knockback)) {
                knockback.InitKnockback(hitDirection, 10f); // Adjust parameters as needed
            }
        } 
    }

    void Die() {

        Instantiate(collectablePrefab, transform.position, Quaternion.identity);
        // // Industry Standard: Never Destroy(). Return to EnemyPool.
        EnemyManager.Instance.RemoveEnemyAtIndex(enemyIndex);
        // // Spawn XP Gem here
        // XPPool.Instance.Spawn(transform.position); 
    }

    // Simple Hit Flash
    System.Collections.IEnumerator FlashEffect() {
        _renderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _renderer.color = Color.red; // Or original color
    }
}
