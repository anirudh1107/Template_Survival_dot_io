using UnityEngine;

public class AreaWeapon : WeaponBase
{

    protected override void Attack() {
        // 1. Find Target
        Transform target = GetNearestEnemy(); 
        if (!target) return; // No enemy? Don't fire, keep cooldown ready? Or waste shot? Depends on design.

        // 2. Calculate Direction
        Vector3 direction = (target.position - projectilePrefab.transform.position).normalized;

        // 3. Spawn Projectile (Using the Pool logic we discussed earlier)
        var p = pool.Get();
        p.transform.position = WeaponManager.Instance.GetPlayerPosition();
        int newIndex  = activeProjectiles.Count;
        p.GetComponent<Projectile>().Init(pool, newIndex, direction, currentStats.speed, currentStats.duration, currentStats.damage, ProjectileType.Area);
        activeProjectiles.Add(p); // Keep track if we need to manage them further (like for area weapons)
        // Ideally: ProjectileManager.Instance.Spawn(projectilePrefab, transform.position, direction, currentStats);
        Debug.Log($"Fired at {target.name}");
    }

    protected override void UpdateProjectiles(float dt) {
        // for (int i = activeProjectiles.Count - 1; i >= 0; i--) {
        //     activeProjectiles[i].GetComponent<Projectile>().ManagedUpdate(dt);
        // }
    }

    Transform GetNearestEnemy() {
        // Reuse the OverlapCircle logic here
        // Ideally this logic is in a centralized "EnemyRadar" script on the player
        // to avoid running OverlapCircle 6 times for 6 different weapons.
        return null;
    }
}
