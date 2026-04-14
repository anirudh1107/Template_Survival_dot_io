using UnityEngine;
using UnityEngine.Pool;

public abstract class Projectile : MonoBehaviour {
    protected IObjectPool<GameObject> _pool;
    protected int _index;
    protected float _speed;
    protected float _timer;
    protected float _lifeTime;
    protected float _damage;

    protected ProjectileType _type;

    // Initialize after getting from pool
    public virtual void Init(IObjectPool<GameObject> pool, int index, Vector3 direction, float speed, float duration, float damage, ProjectileType type) {
        _pool = pool;
        _index = index;
        transform.right = direction; // Assuming 2D
        _speed = speed;
        _lifeTime = duration;
        _damage = damage;
        _timer = 0;
        _type = type;
    }

    public virtual int GetProjectileIndex() {
        return _index;
    }

    public virtual void SetProjectileIndex(int index)
    {
        _index = index;
    }

    public virtual ProjectileType GetProjectileType() {
        return _type;
    }

    // The Manager calls this every frame instead of individual Updates
    public abstract void ManagedUpdate(float deltaTime);
    
}