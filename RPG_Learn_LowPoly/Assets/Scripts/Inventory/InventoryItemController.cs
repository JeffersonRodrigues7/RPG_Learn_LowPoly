using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    Item item;

    public Button RemoveButton;

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
                Debug.Log("AUMENTAR A VIDA DO PLAYER");
                break;
            case Item.ItemType.Ring:
                Debug.Log("AUMENTAR FORÇA DO HIT");
                break;
        }

        RemoveItem();
    }

}
