using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;

    [SerializeField] private Image itemImage;
    public void addItem(string name, Sprite sprite)
    {
        itemName = name;
        itemSprite = sprite;
        isFull = true;
        itemImage.sprite = sprite;
    }
}
