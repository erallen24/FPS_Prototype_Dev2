using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] ObjectMover objectToMove;
    [SerializeField] Renderer objectRenderer;

    [SerializeField] Material activeMaterial;
    [SerializeField] Material idleMaterial;

    private bool playerInRange = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetButtonDown("Interact"))
        {
            objectToMove.ToEndPos();
            if(objectRenderer != null && activeMaterial != null)
            {
                objectRenderer.material = activeMaterial;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
            Gizmos.DrawWireSphere(transform.position, 2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
        }
    }
}
