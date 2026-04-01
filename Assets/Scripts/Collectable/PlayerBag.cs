using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBag : MonoBehaviour
{
    public static Action<float> OnMoneyChanged;
    public static Action<BaseCollectable> OnItemCollected;
    [SerializeField] private float currentMoney = 0f;
    [SerializeField] private float maxMoney = 100f;
    private int maxItemCount = 10;
    private int currentItemCount = 0;
    private BaseCollectable[] collectedItems;

    private void Awake() {
        collectedItems = new BaseCollectable[maxItemCount];
        OnMoneyChanged+= AddMoney;
        OnItemCollected+= CollectItem;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMoney(float amount)
    {
        if (currentMoney + amount > maxMoney)
        {
            currentMoney = maxMoney;
        }
        else
        {
            currentMoney += amount;
        }
        UpgradeManager.Instance.HandleMoneyChange(currentMoney);
        if (currentMoney >= 100f) {
            currentMoney = 0f; // Reset money after triggering upgrade
        }
    }

    public void CollectItem(BaseCollectable item)
    {
        if (currentItemCount < maxItemCount)
        {
            collectedItems[currentItemCount] = item;
            currentItemCount++;
            OnItemCollected?.Invoke(item);
        }
        else
        {
            Debug.Log("Bag is full! Cannot collect more items.");
        }
    }

    public float GetCurrentMoney() {
        return currentMoney;
    }


}
