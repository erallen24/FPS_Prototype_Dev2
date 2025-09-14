using UnityEngine;

public class HealthStation : MonoBehaviour
{
    [SerializeField] Renderer objectRenderer;
    [SerializeField] Material activeMaterial;

    [SerializeField] int healFactor = 5;

    private bool isHealing = false;
    private Material[] materials;
    private Color originalColor;

    void Start()
    {
        originalColor = objectRenderer.material.color;
        materials = objectRenderer.materials;
        objectRenderer = GetComponent<Renderer>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.UpdateInteractPrompt("Hold still to Heal");
            GameManager.instance.interactPromptText.color = Color.green;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.playerScript.isFullyHealed)
            {
                isHealing = false;
                materials[0] = null;
                objectRenderer.materials = materials;
                GameManager.instance.UpdateInteractPrompt("Health Full");
                GameManager.instance.interactPromptText.color = Color.white;
            }

            while (!GameManager.instance.playerScript.isFullyHealed)
            {

                isHealing = true;
                materials[0] = activeMaterial;
                objectRenderer.materials = materials;

                GameManager.instance.playerScript.FillPlayerHPBar(healFactor);


            }
            isHealing = false;


        }


    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.UpdateInteractPrompt("");
        GameManager.instance.interactPromptText.color = Color.white;

        isHealing = false;
        materials[0] = null;
        objectRenderer.materials = materials;

    }
}
