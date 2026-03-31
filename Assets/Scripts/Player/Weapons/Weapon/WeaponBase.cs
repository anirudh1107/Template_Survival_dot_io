using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public abstract class WeaponBase {
    public List<GameObject> activeProjectiles = new List<GameObject>();
    protected WeaponStats currentStats;
    protected float cooldownTimer;
    protected GameObject projectilePrefab;
    protected IObjectPool<GameObject> pool;

    protected int projectileNumber = 1;
    protected float projectileSpeed;
    protected WeaponManager managerContext;

    // Called when weapon is added to player
    public virtual void Initialize(WeaponStats stats, GameObject prefab, WeaponManager managerContext) {
        this.currentStats = stats;
        this.projectilePrefab = prefab;
        pool = WeaponManager.Instance.projectilePools[stats.type];
        this.managerContext = managerContext;
        cooldownTimer = 0;
        this.projectileNumber = 3;
        this.projectileSpeed = stats.speed;
    }

    public virtual void DoUpdate(float dt) {
        cooldownTimer -= dt;
        UpdateProjectiles(dt);
        if (cooldownTimer <= 0) {
            Attack();
            cooldownTimer = currentStats.cooldown;
        }
    }

     public virtual void RemoveProjectileAtIndex(int index)
    {
        if (index < 0 || index >= activeProjectiles.Count) return;

        // 2. Optimized Swap-Back Removal
        int lastIndex = activeProjectiles.Count - 1;
        activeProjectiles[index] = activeProjectiles[lastIndex]; // Move last projectile to current gap
        activeProjectiles[index].GetComponent<Projectile>().SetProjectileIndex(index); // Update the projectile's index in the list
        activeProjectiles.RemoveAt(lastIndex);           // Remove the duplicate at the end
    }

    protected abstract void Attack();
    protected abstract void UpdateProjectiles(float dt);
}


