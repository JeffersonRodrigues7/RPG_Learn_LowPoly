using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Health;
using RPG.Player.Attack;

public class InventoryItemController : MonoBehaviour
{
    Item item; // Representa o item associado a este controlador

    public Button RemoveButton; // Bot�o usado para remover o item do invent�rio
    HealthController health; // Refer�ncia ao controlador de sa�de do jogador
    PlayerAttack force; // Refer�ncia ao controlador de ataque do jogador

    public void Awake()
    {
        // Obt�m as refer�ncias aos controladores de sa�de e ataque do jogador ao acordar
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>();
        force = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
    }

    public void RemoveItem()
    {
        // Remove o item do invent�rio usando o InventoryManager
        InventoryManager.Instance.Remove(item);

        // Destroi o objeto associado ao item
        Destroy(gameObject);
    }

    public void AddItem(Item newItem)
    {
        // Define o item associado a este controlador
        item = newItem;
    }

    public void UseItem()
    {
        // Switch para determinar o tipo de item e executar a a��o apropriada
        switch (item.itemType)
        {
            case Item.ItemType.PotionHealth:
                // Se o item for uma po��o de sa�de, usa a po��o de sa�de no controlador de sa�de
                health.usePotion(0.2f);
                break;
            case Item.ItemType.PotionForceSword:
                // Se o item for uma po��o de aumento de ataque da espada, aumenta o ataque da espada no controlador de ataque
                force.increaseSwordAttack(10f);
                break;
            case Item.ItemType.PotionForceArrow:
                // Se o item for uma po��o de aumento de ataque do arco, aumenta o ataque do arco no controlador de ataque
                force.increaseBowAttack(10f);
                break;
        }

        // Remove o item ap�s us�-lo
        RemoveItem();
    }
}
