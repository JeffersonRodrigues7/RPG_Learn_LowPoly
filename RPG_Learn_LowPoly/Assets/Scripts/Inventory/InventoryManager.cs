using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // Inst�ncia �nica do InventoryManager acess�vel globalmente
    public List<Item> Items = new List<Item>(); // Lista de itens no invent�rio

    public Transform ItemContent; // Conte�do onde os itens do invent�rio ser�o exibidos
    public GameObject InventoryItem; // Modelo de item de invent�rio a ser instanciado

    public Toggle EnableRemove; // Toggle para habilitar/desabilitar a remo��o de itens

    public InventoryItemController[] InventoryItems; // Array de controladores de itens de invent�rio

    private void Awake()
    {
        Instance = this; // Configura a inst�ncia �nica do InventoryManager ao acordar
    }

    public void Add(Item item)
    {
        Items.Add(item); // Adiciona um item � lista de itens no invent�rio
    }

    public void Remove(Item item)
    {
        Items.Remove(item); // Remove um item da lista de itens no invent�rio
    }

    public void ListItems()
    {
        // Limpar invent�rio antes de abastecer novamente
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in Items)
        {
            // Instancia um objeto de item de invent�rio e configura seus elementos visuais
            GameObject obj = Instantiate(InventoryItem, ItemContent);

            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();

            itemName.text = item.itemName; // Define o nome do item
            itemIcon.sprite = item.icon; // Define o �cone do item

            // Ativa o bot�o de remo��o se a op��o estiver habilitada
            if (EnableRemove.isOn)
            {
                removeButton.gameObject.SetActive(true);
            }
        }

        SetInventoryItems(); // Configura os controladores de itens de invent�rio
    }

    public void EnableItemsRemove()
    {
        // Ativa ou desativa os bot�es de remo��o com base no estado do toggle
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
        // Obt�m os controladores de itens de invent�rio associados aos itens existentes
        InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();

        // Adiciona os itens aos controladores correspondentes
        for (int i = 0; i < Items.Count; i++)
        {
            InventoryItems[i].AddItem(Items[i]);
        }
    }
}
