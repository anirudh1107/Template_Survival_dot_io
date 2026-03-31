using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject {
    public string Name;
    public Sprite Icon;
    public WeaponBase behaviourPrefab; // The script that controls logic (Prefab with script)
    public WeaponStats baseStats; // Struct for damage, speed, cooldown, etc.
    public GameObject projectilePrefab; // For weapons that shoot projectiles
}

[System.Serializable]
public struct WeaponStats {
    public float damage;
    public float speed;
    public float cooldown;
    public float duration;
    public float area; // For scale/range
    public int amount; // Number of projectiles
    public ProjectileType type; // Enum for Projectile, Area, Orbital
}
