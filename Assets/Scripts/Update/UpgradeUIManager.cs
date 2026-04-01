using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance { get; private set; }
    [SerializeField] private Canvas upgradeUICanvas;

    [SerializeField] private Image[] upgradeIcons; // Assign in Inspector
    [SerializeField] private TMP_Text[] upgradeDescriptions; // Assign in Inspector

    [SerializeField] private UpgradeManager upgradeManager;

    private List<UpdateData> currentUpgrades; // The 3 upgrades currently being shown


    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

     private void Start() {
        currentUpgrades = new List<UpdateData>();
        upgradeUICanvas.enabled = false; // Start with the UI hidden
    }

     private void Update() {
        
    }

    public void ShowUpgradeUI()
    {
        currentUpgrades.Clear();
        currentUpgrades = upgradeManager.GetRandomUpgrades(3);
        for (int i = 0; i < upgradeIcons.Length; i++)
        {
            if (i < currentUpgrades.Count)
            {
                upgradeIcons[i].sprite = currentUpgrades[i].icon;
                upgradeDescriptions[i].text = $"{currentUpgrades[i].itemName} (Level {currentUpgrades[i].currentLevel}/{currentUpgrades[i].maxLevel})\n{currentUpgrades[i].description}";
                upgradeIcons[i].transform.parent.gameObject.SetActive(true); // Show the icon and description
            }
            else
            {
                upgradeIcons[i].transform.parent.gameObject.SetActive(false); // Hide unused slots
            }
        }
        upgradeUICanvas.enabled = true;
        Time.timeScale = 0f; // Pause the game
    }

     public void OnUpgradeSelected(int index)
    {
        if (index < 0 || index >= currentUpgrades.Count) return;

        upgradeManager.ApplyUpgrade(currentUpgrades[index]);
        upgradeUICanvas.enabled = false;
        Time.timeScale = 1f; // Resume the game
    }

}
