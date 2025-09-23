using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;
    public GameObject highlightItem;
    public bool itemActive;
    private InventoryManager inventoryManager;
    public Image selectedItem;
    public TMP_Text itemDName;
    public TMP_Text itemDescription;

    [SerializeField] private Image itemImage;

    void Start()
    {
        inventoryManager = GameObject.Find("Inventory").GetComponent<InventoryManager>();
    }
    public void addItem(string name, Sprite sprite, string description)
    {
        itemName = name;
        itemSprite = sprite;
        itemDescription.text = description;
        isFull = true;
        itemImage.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inventoryManager.deselectSlots();
            highlightItem.SetActive(true);
            itemActive = true;
        }
    }
}
