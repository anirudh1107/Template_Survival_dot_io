using UnityEngine;
using UnityEngine.UI;

public class HudUiManager : MonoBehaviour
{

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerBag playerBag;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider xpBar;

    void OnEnable()
    {
        playerHealth.OnPlayerHurt += UpdateHealthBar;
        playerHealth.OnPlayerDeath += ShowDeathScreen;
        PlayerBag.OnMoneyChanged += UpdateXpBar;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xpBar.value = 0f; // Initialize XP bar to empty
        healthBar.value = playerHealth.GetCurrentHealth(); // Set max value of health bar to player's max health
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealthBar()
    {
        healthBar.value = playerHealth.GetCurrentHealth(); // Assuming max health is 100
    }

    public void ShowDeathScreen()
    {
        // Implement logic to show death screen or restart level
        Debug.Log("Player has died! Show death screen.");
    }

    public void UpdateXpBar(float currentXp)
    {
        xpBar.value = playerBag.GetCurrentMoney(); // Normalize XP to a value between 0 and 1
    }
}
