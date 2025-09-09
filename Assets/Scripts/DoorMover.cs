using UnityEngine;


public class DoorMover : MonoBehaviour, IInteractable
{
    [SerializeField] float moveDist;
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 moveDir;

    [SerializeField] Renderer objectRenderer;

    [SerializeField] Material activeMaterial;
    [SerializeField] Material idleMaterial;

    [SerializeField] inventoryItem key;

    private Vector3 closedPostion;
    private Vector3 openPostion;

    private bool isOpening = false;
    private bool isLocked = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedPostion = transform.position;
        openPostion = closedPostion + moveDir * moveDist;
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
    private void OnTriggerStay(Collider other)
    {
        if (!isLocked)
        {
            Debug.Log("Press E to Open");
        }
       
    }

    public void Open()
    {
        isOpening = true;
        if (objectRenderer != null && activeMaterial != null)
        {
            objectRenderer.material = activeMaterial;
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
                Debug.Log("Door Unlocked");
                Unlock();
            }
            Debug.Log("Door is locked");
            return;
        }
        else
        {
            Open();
        }
            
    }
}
