using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour {
    private IObjectPool<GameObject> _pool;
    private int _index;
    private float _speed;
    private float _timer;
    private float _lifeTime;
    private float _damage;

    private ProjectileType _type;

    // Initialize after getting from pool
    public void Init(IObjectPool<GameObject> pool, int index, Vector3 direction, float speed, float duration, float damage, ProjectileType type) {
        _pool = pool;
        _index = index;
        transform.right = direction; // Assuming 2D
        _speed = speed;
        _lifeTime = duration;
        _damage = damage;
        _timer = 0;
        _type = type;
    }

    // The Manager calls this every frame instead of individual Updates
    public void ManagedUpdate(float deltaTime) {
        transform.Translate(Vector3.right * _speed * deltaTime);
        _timer += deltaTime;
        
        if (_timer >= _lifeTime) {
            _pool.Release(this.gameObject);
        }
    }

    public int GetProjectileIndex() {
        return _index;
    }

    public void SetProjectileIndex(int index)
    {
        _index = index;
    }

    public ProjectileType GetProjectileType() {
        return _type;
    }
    

    private void OnTriggerEnter2D(Collider2D collision) {

    if (collision.TryGetComponent<Knockback>(out Knockback knockback)) {
        knockback.InitKnockback(transform.right, 10f); // Adjust parameters as needed
    }
    // 2026 Optimization: TryGetComponent is the standard "safe & fast" way
    if (collision.TryGetComponent<IDamageable>(out IDamageable target)) {
        target.TakeDamage(_damage); // We will pass 'damage' from Init()
    }
    if (collision.CompareTag("Enemy")) {
        // Hit something solid, release back to pool
        _pool.Release(this.gameObject);
    }

   
}
}