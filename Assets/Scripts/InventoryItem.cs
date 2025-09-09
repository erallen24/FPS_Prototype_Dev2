using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "InventoryItem")]
public class inventoryItem : ScriptableObject 
{
    [SerializeField] string itemName;
    [SerializeField] Sprite icon;

    
}
