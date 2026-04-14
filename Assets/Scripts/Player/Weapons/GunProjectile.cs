using UnityEngine;

public class GunProjectile : Projectile
{
    private float _knockbackForce = 5f; // Example knockback force, can be set from Init() if needed

    public override void ManagedUpdate(float deltaTime) {
        transform.Translate(Vector3.right * _speed * deltaTime);
        _timer += deltaTime;
        
        if (_timer >= _lifeTime) {
            _pool.Release(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
       
        if (collision.CompareTag("Enemy")) {
             // 2026 Optimization: TryGetComponent is the standard "safe & fast" way
            if (collision.TryGetComponent<IDamageable>(out IDamageable target)) {
                target.TakeDamage(_damage, transform.right, _knockbackForce); // We will pass 'damage' from Init()
            }
            // Hit something solid, release back to pool
            _pool.Release(this.gameObject);
        }
    }
}
