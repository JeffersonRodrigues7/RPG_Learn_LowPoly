using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Health;
using RPG.Player.Attack;

public class InventoryItemController : MonoBehaviour
{
    Item item; // Representa o item associado a este controlador

    public Button RemoveButton; // Botão usado para remover o item do inventário
    HealthController health; // Referência ao controlador de saúde do jogador
    PlayerAttack force; // Referência ao controlador de ataque do jogador

    public void Awake()
    {
        // Obtém as referências aos controladores de saúde e ataque do jogador ao acordar
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthController>();
        force = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
    }

    public void RemoveItem()
    {
        // Remove o item do inventário usando o InventoryManager
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
        // Switch para determinar o tipo de item e executar a ação apropriada
        switch (item.itemType)
        {
            case Item.ItemType.PotionHealth:
                // Se o item for uma poção de saúde, usa a poção de saúde no controlador de saúde
                health.usePotion(0.2f);
                break;
            case Item.ItemType.PotionForceSword:
                // Se o item for uma poção de aumento de ataque da espada, aumenta o ataque da espada no controlador de ataque
                force.increaseSwordAttack(10f);
                break;
            case Item.ItemType.PotionForceArrow:
                // Se o item for uma poção de aumento de ataque do arco, aumenta o ataque do arco no controlador de ataque
                force.increaseBowAttack(10f);
                break;
        }

        // Remove o item após usá-lo
        RemoveItem();
    }
}
