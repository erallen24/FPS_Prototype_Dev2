using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlot;
    private PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public inventoryItem getItem()
    {
        return player.inventory[player.inventory.Count];
    }
    public void addItem(inventoryItem item)
    {
        item = getItem();
        for (int i = 0;  i < inventorySlot.Length; i++)
        {
            if (inventorySlot[i].isFull == false)
            {
                inventorySlot[i].addItem(item);
                return;
            }
        }
    }
    public void deselectSlots()
    {
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            inventorySlot[i].highlightItem.SetActive(false);
            inventorySlot[i].itemActive = false;
        }
    }
}
