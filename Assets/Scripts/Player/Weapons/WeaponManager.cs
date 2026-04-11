using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Linq;
using System;

public class WeaponManager : MonoBehaviour {
    public LayerMask enemyLayer;
    public static WeaponManager Instance { get; private set; }

    public static Action OnGunFire;

    [SerializeField] private WeaponData projectileWeaponData;
    [SerializeField] private WeaponData orbitalWeaponData;
    [SerializeField] private WeaponData areaWeaponData;
    [SerializeField] private Transform weaponPoint;
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
        weaponBehaviors[projectileType].Initialize(data.baseStats, projectileType, weaponDataSODict[projectileType].projectilePrefab, this);
        
        // 4. Add to active list
        activeWeapons.Add(weaponBehaviors[projectileType]);
    }

    void Update() {
    
        float dt = Time.deltaTime;
        for (int i = 0; i < activeWeapons.Count; i++) {
            activeWeapons[i].DoUpdate(dt);
        }
    }

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

    public Vector3 GetWeaponPoint() {
        return weaponPoint.position;
    }

    private WeaponBase GetWeapon(ProjectileType type) {
        if (weaponBehaviors.TryGetValue(type, out var weapon)) {
            return weapon;
        }
        Debug.LogError($"Weapon type {type} not found!");
        return null;
    }

    private void HandleWeaponUpgrade(UpdateData upgradeData) {
        // // For testing, let's just increase projectile number for the projectile weapon
        
        foreach (WeaponBase weapon in activeWeapons) {

            if (weapon.weaponType == upgradeData.weaponType) {
                weapon.Upgrade(upgradeData);
            }
            //Debug.Log($"Active Weapon: {weapon.} with projectile number {weapon.projectileNumber}");
        }
    }

}

public enum ProjectileType {
    Projectile,
    Area,
    Orbital
}