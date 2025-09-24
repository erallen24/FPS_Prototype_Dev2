using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using System;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;
    public GameObject highlightItem;
    public bool itemActive;
    private InventoryManager inventoryManager;
    private ItemIterator itemBar;
    private PlayerController player;
    public Image selectedItem;
    public TMP_Text itemDName;
    public TMP_Text itemDescription;

    [SerializeField] private Image itemImage;

    void Start()
    {
        inventoryManager = GameObject.Find("Inventory").GetComponent<InventoryManager>();
        

    }
    public void addItem(inventoryItem item)
    {
        itemName = item.name;
        //itemSprite = item.icon;
        itemDescription.text = item.description;
        isFull = true;
        //itemImage.sprite = item.icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inventoryManager.deselectSlots();
            highlightItem.SetActive(true);
            itemActive = true;
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //add item to item bar

        }
    }
}
