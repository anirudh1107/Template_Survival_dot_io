using UnityEngine;

public  class BaseCollectable : MonoBehaviour, ICollectable
{
    public CollectableType Type;

    public float Value;
    
    public void OnCollected(Transform collector)
    {
        
        if (Type == CollectableType.Money)
        {
            Debug.Log($"Collected {Value} money.");
            PlayerBag.OnMoneyChanged?.Invoke(Value);
        }
        PlayerBag.OnItemCollected?.Invoke(this);
        
        // This method can be overridden by derived classes to implement specific behavior when collected.
        Debug.Log($"{Type} collected by {collector.name}");
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            OnCollected(other.transform);
            Destroy(gameObject);
        }
    }

    public enum CollectableType
    {
        Health,
        Ammo,
        Food,
        Money
    }
}
