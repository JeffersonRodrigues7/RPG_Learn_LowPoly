using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // Instância única do InventoryManager acessível globalmente
    public List<Item> Items = new List<Item>(); // Lista de itens no inventário

    public Transform ItemContent; // Conteúdo onde os itens do inventário serão exibidos
    public GameObject InventoryItem; // Modelo de item de inventário a ser instanciado

    public Toggle EnableRemove; // Toggle para habilitar/desabilitar a remoção de itens

    public InventoryItemController[] InventoryItems; // Array de controladores de itens de inventário

    private void Awake()
    {
        Instance = this; // Configura a instância única do InventoryManager ao acordar
    }

    public void Add(Item item)
    {
        Items.Add(item); // Adiciona um item à lista de itens no inventário
    }

    public void Remove(Item item)
    {
        Items.Remove(item); // Remove um item da lista de itens no inventário
    }

    public void ListItems()
    {
        // Limpar inventório antes de abastecer novamente
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in Items)
        {
            // Instancia um objeto de item de inventário e configura seus elementos visuais
            GameObject obj = Instantiate(InventoryItem, ItemContent);

            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

            itemName.text = item.itemName; // Define o nome do item
            itemIcon.sprite = item.icon; // Define o ícone do item

            // Ativa o botão de remoção se a opção estiver habilitada
            if (EnableRemove.isOn)
            {
                removeButton.gameObject.SetActive(true);
            }
        }

        SetInventoryItems(); // Configura os controladores de itens de inventário
    }

    public void EnableItemsRemove()
    {
        // Ativa ou desativa os botões de remoção com base no estado do toggle
        if (EnableRemove.isOn)
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(false);
            }
        }
    }

    public void SetInventoryItems()
    {
        // Obtém os controladores de itens de inventário associados aos itens existentes
        InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();

        // Adiciona os itens aos controladores correspondentes
        for (int i = 0; i < Items.Count; i++)
        {
            InventoryItems[i].AddItem(Items[i]);
        }
    }
}
