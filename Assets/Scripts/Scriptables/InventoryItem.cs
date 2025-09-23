using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "InventoryItem")]
public class inventoryItem : ScriptableObject
{
    public float staminaMod;
    public int shieldMod;
    public int armorMod;
    public float EMPTimer;
    public float stunTimer;
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
    [TextArea(16, 10)] public string description;
}
