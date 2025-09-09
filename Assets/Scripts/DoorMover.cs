using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class DoorMover : MonoBehaviour, IInteractable
{
    [SerializeField] float moveDist;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 moveDir;

    [SerializeField] Renderer objectRenderer;
    [SerializeField] Material activeMaterial;
    
    [SerializeField] inventoryItem key;

    private Vector3 closedPostion;
    private Vector3 openPostion;

    private Material[] materials;

    private bool isOpening = false;
    private bool isLocked = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedPostion = transform.position;
        openPostion = closedPostion + moveDir * moveDist;
        materials = objectRenderer.materials;
        objectRenderer = GetComponent<Renderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPostion, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, closedPostion, moveSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isLocked)
            {
                GameManager.instance.UpdateInteractPrompt("Door Unlocked! Press 'E' to Open");
            }
            else if (isLocked && GameManager.instance.playerScript.HasItem(key))
            {
                GameManager.instance.UpdateInteractPrompt("Press 'E' to Unlock Door");
            }
            else if (isLocked)
            {
                GameManager.instance.UpdateInteractPrompt("Door Locked! Find " + key.name + " to Unlock.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.UpdateInteractPrompt("");
    }

    public void Open()
    {
        isOpening = true;
        if (objectRenderer != null && activeMaterial != null)
        {
            materials[2] = activeMaterial;
            objectRenderer.materials = materials;
        }
    }

    public void Close()
    {
        isOpening = false;
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void Interact()
    {
        if (isLocked)
        {
            if (GameManager.instance.playerScript.HasItem(key))
            {
                GameManager.instance.UpdateInteractPrompt("Door Unlocked!");
                Unlock();
                Open();
            }
            return;
        }
        else
        {
            Open();
        }
            
    }
}
