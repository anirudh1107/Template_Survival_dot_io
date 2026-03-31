using System;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    
    [SerializeField] private float knockbackDuration = 0.2f;

    public Action OnKnockbackStart;
    public Action OnKnockbackEnd; // Event to signal when knockback is done

    private Rigidbody2D rb;
    private float knockbackForce;
    private Vector2 knockbackDirection;

    void OnEnable()
    {
        OnKnockbackStart += ApplyKnockback;
        OnKnockbackEnd += StopKnockBack;
    }
    void Awake() => rb = GetComponent<Rigidbody2D>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitKnockback(Vector2 direction, float knockbackForce) {
        this.knockbackDirection = direction.normalized;
        this.knockbackForce = knockbackForce;

        OnKnockbackStart?.Invoke();
    }

    public void ApplyKnockback() {

        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(KnockbackCoroutine(knockbackDirection, knockbackDuration));
    }

    private void StopKnockBack() {
        rb.linearVelocity = Vector2.zero; // Stop movement immediately
    }

    private System.Collections.IEnumerator KnockbackCoroutine(Vector2 direction, float duration)
    {
  
        yield return new WaitForSeconds(duration);
        OnKnockbackEnd?.Invoke();
    }

}
