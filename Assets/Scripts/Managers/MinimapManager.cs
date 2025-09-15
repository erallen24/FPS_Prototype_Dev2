
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MinimapManager : MonoBehaviour
{

    public static MinimapManager instance; // Singleton instance for easy access

    [SerializeField] private GameObject minimapCanvas;
    [SerializeField] private Camera minimapCam;
    [SerializeField] private Image reticalImage;
    public List<XReModule> collectedModules = new List<XReModule>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        minimapCanvas.SetActive(false);
    }


    public void ActivateModule(XReModule moduleData)
    {
        minimapCanvas.SetActive(true);
        // Check if the module has already been collected
        if (collectedModules.Contains(moduleData))
        {
            Debug.Log("Module " + moduleData.moduleName + " has already been collected.");
            foreach (string layer in moduleData.layersToEnable)
            {

                minimapCam.cullingMask |= LayerMask.GetMask(layer);
            }

            foreach (GameObject overlay in moduleData.overlayObjects)
            {
                if (overlay != null) overlay.SetActive(true);
            }

            if (!string.IsNullOrEmpty(moduleData.activationMessage))
                HUDManager.instance.UpdateInteractPrompt(moduleData.activationMessage);

            if (moduleData.activationSound != null)
                AudioSource.PlayClipAtPoint(moduleData.activationSound, transform.position);

        }
        else if (!collectedModules.Contains(moduleData))
        {
            collectedModules.Add(moduleData);

            foreach (string layer in moduleData.layersToEnable)
            {
                // Enable the specified layer in the minimap camera's culling mask
                minimapCam.cullingMask |= LayerMask.GetMask(layer); // Unset the bit to show the layer


            }

            foreach (GameObject overlay in moduleData.overlayObjects)
            {
                if (overlay != null) overlay.SetActive(true);
            }

            if (!string.IsNullOrEmpty(moduleData.activationMessage))
                HUDManager.instance.ShowPromptTemporary(moduleData.activationMessage, 3);

            if (moduleData.activationSound != null)
                SoundManager.instance.soundSource.PlayOneShot(moduleData.activationSound);
        }

    }

    public void DeactivateModule(XReModule moduleData, Camera minimapCam)
    {
        // Check if the module has already been collected
        if (collectedModules.Contains(moduleData))
        {
            foreach (string layer in moduleData.layersToEnable)
            {
                // Enable the specified layer in the minimap camera's culling mask
                minimapCam.cullingMask &= ~LayerMask.GetMask(layer);

            }

            foreach (GameObject overlay in moduleData.overlayObjects)
            {
                if (overlay != null) overlay.SetActive(false);
            }

        }
    }
}

