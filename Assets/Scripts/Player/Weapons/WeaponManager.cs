using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Linq;

public class WeaponManager : MonoBehaviour {
    public LayerMask enemyLayer;
    public static WeaponManager Instance { get; private set; }

    [SerializeField] private WeaponData projectileWeaponData;
    [SerializeField] private WeaponData orbitalWeaponData;
    [SerializeField] private WeaponData areaWeaponData;
    public Dictionary<ProjectileType, WeaponBase> weaponBehaviors = new Dictionary<ProjectileType, WeaponBase>();
    public Dictionary<ProjectileType, IObjectPool<GameObject>> projectilePools = new Dictionary<ProjectileType, IObjectPool<GameObject>>();
    public Dictionary<ProjectileType, WeaponData> weaponDataSODict = new Dictionary<ProjectileType, WeaponData>();
    
    private WeaponBase projectileWeapon;
    private WeaponBase orbitalWeapon;
    private WeaponBase areaWeapon;
    private List<Collider2D> enemyResults = new List<Collider2D>();
    // private IObjectPool<Projectile> _pool;
    // private List<Projectile> _activeProjectiles = new List<Projectile>();
    // private float _fireTimer;

    private List<WeaponBase> activeWeapons = new List<WeaponBase>();

    void OnEnable()
    {
        UpgradeManager.OnWeaponUpgrade += HandleWeaponUpgrade;
    }

    void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

       
       if (projectileWeaponData != null) weaponDataSODict.Add(ProjectileType.Projectile, projectileWeaponData);
       if (orbitalWeaponData != null) weaponDataSODict.Add(ProjectileType.Orbital, orbitalWeaponData);
       if (areaWeaponData != null) weaponDataSODict.Add(ProjectileType.Area, areaWeaponData);
        projectileWeapon = new ProjectileWeapon();
        orbitalWeapon = new OrbitalWeapon();
        areaWeapon = new AreaWeapon();
        weaponBehaviors.Add(ProjectileType.Projectile, projectileWeapon);
        weaponBehaviors.Add(ProjectileType.Orbital, orbitalWeapon);
        weaponBehaviors.Add(ProjectileType.Area, areaWeapon);
        
        // Create the pool: (Create, OnGet, OnRelease, OnDestroy)
        // _pool = new ObjectPool<Projectile>(
        //     CreateProjectile, 
        //     p => { p.gameObject.SetActive(true); _activeProjectiles.Add(p); }, 
        //     p => { p.gameObject.SetActive(false); _activeProjectiles.Remove(p); }, 
        //     p => Destroy(p.gameObject), 
        //     false, 10, 1000);
    }

    private void Start() {
        // For testing, we can add all weapons at start
        AddWeapon(ProjectileType.Projectile, weaponDataSODict[ProjectileType.Projectile]);
    }

    private void CreatePool(ProjectileType type) {
        if (projectilePools.ContainsKey(type)) return;

        GameObject prefab = weaponDataSODict[type].projectilePrefab;
        projectilePools.Add(type, new ObjectPool<GameObject>(
                () => Instantiate(prefab), 
                p => { p.gameObject.SetActive(true); }, 
                p => DeactivateProjectile(p.GetComponent<Projectile>()), 
                p => Destroy(p.gameObject), 
                false, 20, 200));
    }

    public void DeactivateProjectile(Projectile projectile) {
        weaponBehaviors[projectile.GetProjectileType()].RemoveProjectileAtIndex(projectile.GetProjectileIndex());
        projectile.gameObject.SetActive(false);
    }

    public void AddWeapon(ProjectileType projectileType, WeaponData data) {
        
        CreatePool(projectileType);
        // 3. Initialize with stats
        weaponBehaviors[projectileType].Initialize(data.baseStats, weaponDataSODict[projectileType].projectilePrefab, this);
        
        // 4. Add to active list
        activeWeapons.Add(weaponBehaviors[projectileType]);
    }

    // Projectile CreateProjectile() {
    //      var p = Instantiate(weaponDataSODict[ProjectileType.Projectile].projectilePrefab).GetComponent<Projectile>();
    //      return p;
    // }

    void Update() {
        // // 1. Handle Firing Cooldown
        // _fireTimer -= Time.deltaTime;
        // if (_fireTimer <= 0) {
        //     Fire();
        //     _fireTimer = data.baseStats.cooldown;
        // }

        // // 2. Optimized Movement: One loop for all bullets
        // float dt = Time.deltaTime;
        // for (int i = _activeProjectiles.Count - 1; i >= 0; i--) {
        //     _activeProjectiles[i].ManagedUpdate(dt);
        // }

        // Run all weapons
        float dt = Time.deltaTime;
        for (int i = 0; i < activeWeapons.Count; i++) {
            activeWeapons[i].DoUpdate(dt);
        }
    }

    // void Fire() {
    //     Transform target = GetNearestEnemy();
    //     if (target == null) return;

    //     Vector3 dir = (target.position - transform.position).normalized;
    //     var p = _pool.Get();
    //     p.transform.position = transform.position;
    //     p.Init(_pool, dir, data.baseStats.speed, 5f, data.baseStats.damage);
    // }

    // public Transform GetNearestEnemy(float range) {
    //     // Standard optimization: Only check within range
    //     Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.position, range, enemyLayer);
    //     Transform closest = null;
    //     float minSqDist = Mathf.Infinity;

    //     foreach (var hit in hits) {
    //         float sqDist = (hit.transform.position - this.transform.position).sqrMagnitude;
    //         if (sqDist < minSqDist) {
    //             minSqDist = sqDist;
    //             closest = hit.transform;
    //         }
    //     }
    //     return closest;
    // }

    public List<Transform> GetEnemiesByDistance(float range) {

        // Sort the list in place using a comparison delegate
        return Physics2D.OverlapCircleAll(transform.position, range, enemyLayer)
        .OrderBy(h => (h.transform.position - transform.position).sqrMagnitude)
        .Select(h => h.transform)
        .ToList();
}
    public Vector3 GetPlayerPosition() {
        return this.transform.position;
    }   

    private WeaponBase GetWeapon(ProjectileType type) {
        if (weaponBehaviors.TryGetValue(type, out var weapon)) {
            return weapon;
        }
        Debug.LogError($"Weapon type {type} not found!");
        return null;
    }

    private void HandleWeaponUpgrade() {
        // // For testing, let's just increase projectile number for the projectile weapon
        // foreach (WeaponBase weapon in activeWeapons) {
        //     Debug.Log($"Active Weapon: {weapon.Name} with projectile number {weapon.projectileNumber}");
        // }
        // if (weaponBehaviors.TryGetValue(ProjectileType.Projectile, out var weapon)) {
        //     weapon.projectileNumber += 1; // This will affect how many projectiles are fired in the next attack
        //     Debug.Log($"Upgraded Projectile Weapon! Now fires {weapon.projectileNumber} projectiles.");
        // }
    }

}

public enum ProjectileType {
    Projectile,
    Area,
    Orbital
}