using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "InventoryItem")]
public class inventoryItem : ScriptableObject
{
    public enum ItemType
    {
        AdrenalineShot,
        Shield,
        EMP,
        CloakingDevice,
        StunGrenade,
        Bomb,
        Weapon,
        Armor,
        Accessory,
        Misc
    }
    public ItemType itemType;
    public string itemName;
    [SerializeField] Sprite icon;

}
