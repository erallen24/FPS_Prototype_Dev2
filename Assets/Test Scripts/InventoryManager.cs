using TMPro;
using UnityEngine;
using static InventoryItem;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public ItemType[] itemTypes;
    // A list of inventory items
    public InventoryItem[] inventoryItems;
    public InventoryItem selectedItem = null;

    public Sprite selectedItemIcon = null;
    public TMP_Text selectedItemName = null;

    public TMP_Text itemCountDisplay = null;
    public TMP_Text inventorySizeDisplay = null;

    public int maxItems = 20;
    public int maxInventorySize = 100;
    public int currentInventorySize = 0;
    //public int inventoryWeight = 0;
    //public int maxInventoryWeight = 100;
    public int itemCount = 0;
    public int totalItemCount = 0;
    public int maxItemCount = 100;
    public int experiencePoints = 0;
    public int level = 1;
    public int experienceToNextLevel = 100;

    public int itemAmount;
    public int selectedItemIndex = -1;
    public int selectedItemQuantity = 0;

    public bool isInventoryFull = false;
    public bool isItemSelected = false;
    public bool isUsingItem = false;
    public bool isDroppingItem = false;
    public bool isAddingItem = false;
    public bool isRemovingItem = false;
    public bool isEquippingItem = false;
    public bool isUnequippingItem = false;
    // public int strength = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        instance = this;
        resetAll();
        DontDestroyOnLoad(this.gameObject);

        itemTypes = (ItemType[])System.Enum.GetValues(typeof(ItemType));

    }
    void Start()
    {
        // Example items to add to the inventory
        // AddItem(new InventoryItem { Type = ItemType.AdrenalineShot, Name = "Adrenaline Shot", Icon = null });
        // AddItem(new InventoryItem { Type = ItemType.Shield, Name = "Shield", Icon = null });
        // AddItem(new InventoryItem { Type = ItemType.EMP, Name = "EMP", Icon = null });
    }
    private void Update()
    {

    }
    public bool AddItem(InventoryItem item)
    {
        if (itemCount >= maxItems || currentInventorySize >= maxInventorySize)
        {
            isInventoryFull = true;
            Debug.Log("Inventory is full!");
            return false;
        }
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
            {
                inventoryItems[i] = item;
                itemCount++;
                totalItemCount++;
                currentInventorySize++;
                isInventoryFull = false;
                Debug.Log($"Added {item.Name} to inventory.");
                return true;
            }
        }
        isInventoryFull = true;
        Debug.Log("No empty slot found, inventory is full!");
        return false;
    }
    public bool RemoveItem(InventoryItem item)
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == item)
            {
                inventoryItems[i] = null;
                itemCount--;
                currentInventorySize--;
                //inventoryWeight -= 1; // Assuming each item has a weight of 1 for simplicity
                Debug.Log($"Removed {item.Name} from inventory.");
                return true;
            }
        }
        Debug.Log($"{item.Name} not found in inventory.");
        return false;
    }
    public void SelectItem(int index)
    {
        if (index >= 0 && index < inventoryItems.Length && inventoryItems[index] != null)
        {
            selectedItem = inventoryItems[index];
            selectedItemIndex = index;
            isItemSelected = true;
            Debug.Log($"Selected {selectedItem.Name}.");
        }
        else
        {
            selectedItem = null;
            selectedItemIndex = -1;
            isItemSelected = false;
            Debug.Log("No item selected.");
        }
    }
    public void UseSelectedItem()
    {
        if (isItemSelected && selectedItem != null)
        {
            isUsingItem = true;
            Debug.Log($"Using {selectedItem.Name}.");
            // Implement item usage logic here
            isUsingItem = false;
        }
        else
        {
            Debug.Log("No item selected to use.");
        }
    }
    public void DropSelectedItem()
    {
        if (isItemSelected && selectedItem != null)
        {
            isDroppingItem = true;
            Debug.Log($"Dropping {selectedItem.Name}.");
            RemoveItem(selectedItem);
            selectedItem = null;
            selectedItemIndex = -1;
            isItemSelected = false;
            isDroppingItem = false;
        }
        else
        {
            Debug.Log("No item selected to drop.");
        }
    }
    // Method to gain experience points and handle leveling up
    public void GainExperience(int amount)
    {
        experiencePoints += amount;
        Debug.Log($"Gained {amount} XP. Total XP: {experiencePoints}/{experienceToNextLevel}");
        if (experiencePoints >= experienceToNextLevel)
        {
            LevelUp();
        }
    }
    private void LevelUp()
    {
        level++;
        experiencePoints -= experienceToNextLevel;
        experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f); // Increase XP needed for next level
        Debug.Log($"Leveled up! New level: {level}. XP for next level: {experienceToNextLevel}");
        // Optionally, increase player stats here
    }
    public void resetAll()
    {
        inventoryItems = new InventoryItem[maxItems];
        itemCount = 0;
        totalItemCount = 0;
        currentInventorySize = 0;
        //inventoryWeight = 0;
        experiencePoints = 0;
        level = 1;
        experienceToNextLevel = 100;
        selectedItem = null;
        selectedItemIndex = -1;
        isItemSelected = false;
        isInventoryFull = false;
    }
    // Sort inventory items by type
    public void SortInventoryByType()
    {
        System.Array.Sort(inventoryItems, (item1, item2) =>
        {
            if (item1 == null && item2 == null) return 0;
            if (item1 == null) return 1;
            if (item2 == null) return -1;
            return item1.Type.CompareTo(item2.Type);
        });
        Debug.Log("Inventory sorted by item type.");
    }
    // Print inventory content with icon, names, and types for UI display
    public void PrintInventory()
    {
        Debug.Log("Inventory Contents:");
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] != null)
            {
                Debug.Log($"Slot {i + 1}: {inventoryItems[i].Name} (Type: {inventoryItems[i].Type})");
            }
            else
            {
                Debug.Log($"Slot {i + 1}: Empty");
            }
        }
    }

}



//    // Update is called once per frame
//    void Update()
//    {

//    }
//}
