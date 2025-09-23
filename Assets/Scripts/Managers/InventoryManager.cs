using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addItem(string name, Sprite sprite)
    {
        for (int i = 0;  i < inventorySlot.Length; i++)
        {
            if (inventorySlot[i].isFull == false)
            {
                inventorySlot[i].addItem(name, sprite);
                return;
            }
        }
    }
}
