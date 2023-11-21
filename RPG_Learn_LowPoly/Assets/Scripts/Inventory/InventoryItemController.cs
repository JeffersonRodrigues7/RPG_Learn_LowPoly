using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Health;

public class InventoryItemController : MonoBehaviour
{
    Item item;

    public Button RemoveButton;


    HealthController health = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>();

    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(item);

        Destroy(gameObject);
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
    }

    public void UseItem()
    {
        
        switch (item.itemType)
        {
            case Item.ItemType.PotionHealth:
                health.usePotion(0.2f);
                ;
                break;
            case Item.ItemType.PotionForce:
                Debug.Log("AUMENTAR FORÇA DO HIT");
                break;
        }

        RemoveItem();
    }

}
