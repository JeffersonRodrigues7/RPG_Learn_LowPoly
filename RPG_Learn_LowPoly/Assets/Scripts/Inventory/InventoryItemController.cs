using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Health;
using RPG.Player.Attack;

public class InventoryItemController : MonoBehaviour
{
    Item item;

    public Button RemoveButton;
    HealthController health;
    PlayerAttack force;

    public void Awake()
    {   
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>();
        force = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
    }

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
                break;
            case Item.ItemType.PotionForceSword:
                force.increaseSwordAttack(10f);
                break;
            case Item.ItemType.PotionForceArrow:
                force.increaseBowAttack(10f);
                break;
        }

        RemoveItem();
    }

}
