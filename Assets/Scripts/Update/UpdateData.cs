using UnityEngine;

public enum UpdateType { Weapon, Passive, Consumable }
[CreateAssetMenu(fileName = "UpdateData", menuName = "Scriptable Objects/UpdateData")]
public class UpdateData : ScriptableObject
{
     public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public UpdateType itemType;
    public ProjectileType weaponType; // Only relevant if itemType is Weapon
    
    [Header("Progression")]
    public int currentLevel = 1;
    public int maxLevel = 5;
    
    // Add specific stats here (e.g., damage modifier, speed boost, etc.)
    public float baseDamage;
    public float projectileNumber;
}
