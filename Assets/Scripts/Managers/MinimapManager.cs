

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MinimapManager : MonoBehaviour
{

    public static MinimapManager instance; // Singleton instance for easy access

    [Header("Minimap Components")]
    [SerializeField] private GameObject minimapCanvas;

    public Camera minimapCam;
    [SerializeField] private Image reticalImage;

    [Header("Module Overlays")]
    [SerializeField] GameObject[] tacticalOveralys;
    [SerializeField] GameObject[] environmentalOverlays;
    [SerializeField] GameObject[] supportOverlays;
    [SerializeField] GameObject[] offensiveOverlays;
    [SerializeField] GameObject[] defensiveOverlays;
    [SerializeField] GameObject[] reconOverlays;
    [SerializeField] GameObject[] temporalOverlays;
    [SerializeField] GameObject[] stabilizationOverlays;
    [SerializeField] GameObject[] experimentalOverlays;


    [Header("Minimap Layer Activation")]
    [SerializeField] string[] tacticalLayers;
    [SerializeField] string[] environmentalLayers;
    [SerializeField] string[] supportLayers;
    [SerializeField] string[] offensiveLayers;
    [SerializeField] string[] defensiveLayers;
    [SerializeField] string[] reconLayers;
    [SerializeField] string[] temporalLayers;
    [SerializeField] string[] stabilizationLayers;
    [SerializeField] string[] experimentalLayers;


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

            EnableModuleFeatures(moduleData);


            if (!string.IsNullOrEmpty(moduleData.activationMessage))
                HUDManager.instance.UpdateInteractPrompt(moduleData.activationMessage);

            if (moduleData.activationSound != null)
                AudioSource.PlayClipAtPoint(moduleData.activationSound, transform.position);

        }
        else if (!collectedModules.Contains(moduleData))
        {
            collectedModules.Add(moduleData);

            EnableModuleFeatures(moduleData);


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



        }
    }

    // Switchcase for module types to enable specific layers or overlays based on module type
    private void EnableModuleFeatures(XReModule moduleData)
    {
        switch (moduleData.moduleType)
        {
            case XReModule.ModuleType.Tactical:
                // Enable tactical layers or overlays
                ActivateTactical();
                reticalImage.sprite = moduleData.reticalImage;
                reticalImage.enabled = true;
                break;
            case XReModule.ModuleType.Environmental:
                // Enable environmental layers or overlays
                break;
            case XReModule.ModuleType.Support:
                // Enable support layers or overlays
                break;
            case XReModule.ModuleType.Offensive:
                // Enable offensive layers or overlays
                break;
            case XReModule.ModuleType.Defensive:
                // Enable defensive layers or overlays
                break;
            case XReModule.ModuleType.Recon:
                // Enable recon layers or overlays
                break;
            case XReModule.ModuleType.Temporal:
                // Enable temporal layers or overlays
                break;
            case XReModule.ModuleType.Stabilization:
                // Enable stabilization layers or overlays
                break;
            case XReModule.ModuleType.Experimental:
                // Enable experimental layers or overlays
                break;
            default:
                Debug.LogWarning("Unknown module type: " + moduleData.moduleType);
                break;
        }
    }
    private void ActivateTactical()
    {
        foreach (GameObject overlay in tacticalOveralys)
        {
            if (overlay != null) overlay.SetActive(true);
        }
        minimapCanvas.SetActive(true);

        foreach (string layerName in tacticalLayers)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer != -1)
            {
                minimapCam.cullingMask |= (1 << layer);
            }
            else
            {
                Debug.LogWarning("Layer not found: " + layerName);
            }
        }
    }


}

