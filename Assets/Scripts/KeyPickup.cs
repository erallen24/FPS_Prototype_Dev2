using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [SerializeField] inventoryItem key;

    public void Interact()
    {
        Debug.Log("Key Found");
        GameManager.instance.playerScript.AddItem(key);
        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
