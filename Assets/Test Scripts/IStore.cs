using UnityEngine;
using static InventoryItem;

public interface IStore
{
    void addItemToInventory(ItemType type, string itemID, int quantity, GameObject obj);
}
