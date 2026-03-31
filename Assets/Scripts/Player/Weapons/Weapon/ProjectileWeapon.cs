using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ProjectileWeapon : WeaponBase { 

    private List<Transform> enemyTargets = new List<Transform>();

    protected override void Attack() {
        // 1. Find Target
        List<Transform> targets = GetNearestEnemy(projectileNumber); 
        if (targets.Count == 0) return; // No enemy? Don't fire, keep cooldown ready? Or waste shot? Depends on design.

        // 2. Calculate Direction

        // 3. Spawn Projectile (Using the Pool logic we discussed earlier)
        for (int i = 0; i < projectileNumber; i++) {
            if (i >= targets.Count) break; // If we have fewer targets than projectileNumber, just fire at available targets.
            Vector3 direction = (targets[i].position - managerContext.transform.position).normalized;
            var p = pool.Get();
            p.transform.position = WeaponManager.Instance.GetPlayerPosition();
            p.GetComponent<Projectile>().Init(pool, activeProjectiles.Count, direction, projectileSpeed, currentStats.duration, currentStats.damage, ProjectileType.Projectile);
            activeProjectiles.Add(p); // Keep track if we need to manage them further (like for area weapons)
            // Ideally: ProjectileManager.Instance.Spawn(projectilePrefab, transform.position, direction, currentStats);
            Debug.Log($"Fired at {targets[i].name}"); 
        }
        
    }

    protected override void UpdateProjectiles(float dt) {
        for (int i = activeProjectiles.Count - 1; i >= 0; i--) {
            activeProjectiles[i].GetComponent<Projectile>().ManagedUpdate(dt);
        }
    }

    List<Transform> GetNearestEnemy(int numberOfTargets = 1) {
        // Reuse the OverlapCircle logic here
        // Ideally this logic is in a centralized "EnemyRadar" script on the player
        // to avoid running OverlapCircle 6 times for 6 different weapons.
        enemyTargets.Clear();
        enemyTargets = WeaponManager.Instance.GetEnemiesByDistance(currentStats.area);
        int i =0;
        foreach (var enemy in enemyTargets) {
            Debug.Log($"Found enemy: {i} at distance {enemy.position}");
            i++;
        }
        return enemyTargets.Take(numberOfTargets).ToList(); // Return the closest N targets
    }
}
