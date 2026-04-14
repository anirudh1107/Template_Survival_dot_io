using UnityEngine;

public class GunProjectile : Projectile
{

    public override void ManagedUpdate(float deltaTime) {
        transform.Translate(Vector3.right * _speed * deltaTime);
        _timer += deltaTime;
        
        if (_timer >= _lifeTime) {
            _pool.Release(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // 2026 Optimization: TryGetComponent is the standard "safe & fast" way
        if (collision.TryGetComponent<IDamageable>(out IDamageable target)) {
            target.TakeDamage(_damage, transform.right); // We will pass 'damage' from Init()
        }
        if (collision.CompareTag("Enemy")) {
            // Hit something solid, release back to pool
            _pool.Release(this.gameObject);
        }
    }
}
