using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [SerializeField] inventoryItem key;

    [SerializeField] int rotationSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.UpdateInteractPrompt("Press 'E' to pick up " + key.itemName);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.UpdateInteractPrompt("");
    }

    public void Interact()
    {
        Debug.Log("Key Found");
        GameManager.instance.playerScript.AddItem(key);
        Destroy(gameObject);
        GameManager.instance.UpdateInteractPrompt("");
    }

}
