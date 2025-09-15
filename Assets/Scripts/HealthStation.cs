using System.Collections;
using UnityEngine;

public class HealthStation : MonoBehaviour, IInteractable
{
    [SerializeField] Renderer objectRenderer;

    [SerializeField] Material activeMaterial;
    [SerializeField] int healAmount = 1;
    private bool isHealing = false;
    private Material origMaterial;
    //private Color originalColor;

    void Start()
    {
        origMaterial = objectRenderer.material;
        objectRenderer = GetComponent<Renderer>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HUDManager.instance.UpdateInteractPrompt("Hold E to Heal...");
            HUDManager.instance.interactPromptText.color = Color.red;
        }
    }

    public void Interact()
    {
        if (!GameManager.instance.playerScript.isFullyHealed)
        {

            isHealing = true;
            StartCoroutine(HealOverTime());


        }

        if (GameManager.instance.playerScript.isFullyHealed)
        {
            isHealing = false;
            HUDManager.instance.UpdateInteractPrompt("Fully Healed!");
            HUDManager.instance.interactPromptText.color = Color.green;
            objectRenderer.material = origMaterial;
        }
    }



    private void OnTriggerExit(Collider other)
    {
        HUDManager.instance.UpdateInteractPrompt("");
        HUDManager.instance.interactPromptText.color = Color.white;

        isHealing = false;

        objectRenderer.material = origMaterial;

    }

    private IEnumerator HealOverTime()
    {
        objectRenderer.material = activeMaterial;
        GameManager.instance.playerScript.FillPlayerHPBar(healAmount);
        yield return new WaitForSeconds(1f);

    }


}
