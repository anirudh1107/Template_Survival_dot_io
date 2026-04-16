using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public Action OnPlayerHurt;
    public Action OnPlayerDeath;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float damageCooldown = 1f; // Time in seconds between taking damage
    [SerializeField] private float lastDamageTime = -Mathf.Infinity;
    [SerializeField] private float defaultDmage = 10f; 


     void OnEnable()
    {
        // Subscribe to events if needed
    }

    private void Awake() {
        currentHealth = maxHealth;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        if (Time.time - lastDamageTime < damageCooldown) return; // Still in cooldown, ignore damage

        currentHealth -= amount;
        lastDamageTime = Time.time;

        OnPlayerHurt?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

     private void Die()
    {
        OnPlayerDeath?.Invoke();
        // Add death logic here (e.g., play animation, disable player controls, etc.)
        Debug.Log("Player has died!");
        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {
            Debug.Log("Player hit by enemy!");
            TakeDamage(defaultDmage); // You can replace this with actual damage from the projectile
            this.GetComponent<Knockback>().ApplyKnockback(); // Apply knockback effect
        }
    }

   
}
