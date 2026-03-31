using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public static Action OnWeaponUpgrade;
    public static Action OnPlayerUpgrade;
    

    [SerializeField] private List<UpdateData> allAvailableItems; // Assign in Inspector
    private List<UpdateData> currentRunItems;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Clone the list for the current run so we don't modify the master list
        currentRunItems = new List<UpdateData>(allAvailableItems);
    }

    // Call this to get 3 unique random upgrades
    public List<UpdateData> GetRandomUpgrades(int count = 3)
    {
        // Filter out max level items
        var validItems = currentRunItems.Where(item => item.currentLevel < item.maxLevel).ToList();

        if (validItems.Count == 0) return null; // Player is fully maxed out!

        // Shuffle the valid items (Fisher-Yates style)
        for (int i = 0; i < validItems.Count; i++)
        {
            UpdateData temp = validItems[i];
            int randomIndex = UnityEngine.Random.Range(i, validItems.Count);
            validItems[i] = validItems[randomIndex];
            validItems[randomIndex] = temp;
        }

        // Return the requested amount, or whatever is left
        return validItems.Take(Mathf.Min(count, validItems.Count)).ToList();
    }

    // Call this when the player selects an upgrade
    public void ApplyUpgrade(UpdateData selectedUpgrade)
    {
        // Find the matching item in the current run list
        UpdateData item = currentRunItems.FirstOrDefault(i => i.itemName == selectedUpgrade.itemName);
        if (item != null)
        {
            item.currentLevel++; // Increment the level

            // Trigger events based on type
            if (item.itemType == UpdateType.Weapon) OnWeaponUpgrade?.Invoke();
            else if (item.itemType == UpdateType.Passive) OnPlayerUpgrade?.Invoke();
        }
    }
}