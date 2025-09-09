using UnityEngine;


public class KeyItem : MonoBehaviour, IInteractable
{
    [SerializeField] inventoryItem key;

    public void Interact()
    {
        Debug.Log("Key Foound");
        GameManager.instance.playerScript.AddItem(key);
        Destroy(gameObject);

    }

    
}
