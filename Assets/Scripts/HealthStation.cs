using System.Collections;
using UnityEngine;

public class HealthStation : MonoBehaviour
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
            GameManager.instance.UpdateInteractPrompt("Hold still to Heal...");
            GameManager.instance.interactPromptText.color = Color.red;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (!GameManager.instance.playerScript.isFullyHealed)
            {

                isHealing = true;
                StartCoroutine(HealOverTime());


            }

            if (GameManager.instance.playerScript.isFullyHealed)
            {
                isHealing = false;
                GameManager.instance.UpdateInteractPrompt("Fully Healed!");
                GameManager.instance.interactPromptText.color = Color.green;
                objectRenderer.material = origMaterial;
            }



        }


    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.UpdateInteractPrompt("");
        GameManager.instance.interactPromptText.color = Color.white;

        isHealing = false;

        objectRenderer.material = origMaterial;

    }

    private IEnumerator HealOverTime()
    {
        while (isHealing && !GameManager.instance.playerScript.isFullyHealed)
        {
            objectRenderer.material = activeMaterial;
            GameManager.instance.playerScript.FillPlayerHPBar(healAmount);
            yield return new WaitForSeconds(1f);
        }

        objectRenderer.material = origMaterial;
        isHealing = false;
    }
}
