using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public static Action<UpdateData> OnWeaponUpgrade;
    public static Action<UpdateData> OnPlayerUpgrade;
    

    [SerializeField] private List<UpdateData> allAvailableItems; // Assign in Inspector
    private List<UpdateData> currentRunItems;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Clone the list for the current run so we don't modify the master list
        currentRunItems = new List<UpdateData>(allAvailableItems);
    }

    private void Start() {
        foreach (var item in allAvailableItems) {
            item.currentLevel = 1; // Reset levels at the start of each run
        }
    }

    public void TriggerUpgrade()
    {
        UpgradeUIManager.Instance.ShowUpgradeUI();
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
        if (selectedUpgrade != null)
        {
            selectedUpgrade.currentLevel++; // Increment the level

            // Trigger events based on type
            if (selectedUpgrade.itemType == UpdateType.Weapon) OnWeaponUpgrade?.Invoke(selectedUpgrade);
            else if (selectedUpgrade.itemType == UpdateType.Passive) OnPlayerUpgrade?.Invoke(selectedUpgrade);
        }
    }

    public void HandleMoneyChange(float newAmount) {
        // Example: Trigger upgrade every time player reaches a multiple of 100 money
        if (newAmount >= 100f) {
            TriggerUpgrade();
        }

    }
}