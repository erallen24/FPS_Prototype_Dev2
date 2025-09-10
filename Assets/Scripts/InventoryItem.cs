using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "InventoryItem")]
public class inventoryItem : ScriptableObject 
{
    public string itemName;
    [SerializeField] Sprite icon;

    
}
