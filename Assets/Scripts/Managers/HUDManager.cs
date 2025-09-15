using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private GameObject retical;
    [SerializeField] private Image reticalImage;
    [SerializeField] private GameObject playerUIBars;


    public Image playerHPBar;
    // public Image playerRageBar;
    public Image playerStaminaBar;
    public Image playerEXPBar;
    public GameObject playerAmmoCanvas;
    public Image playerAmmoBar;
    public Image playerBulletImage;
    public GameObject bossHPBar;
    public Image bossHPBarFill;
    public TMP_Text bossNameText;

    [SerializeField] private GameObject minimapCanvas;
    [SerializeField] private Camera minimapCam;


    public int playerLevel = 0;
    [SerializeField] TMP_Text levelHUDText;
    [SerializeField] GameObject levelHUDFeedback;
    [SerializeField] TMP_Text levelHUDFeedbackText;


    [SerializeField] TMP_Text playerAmmo;
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text keyCountText;

    public TMP_Text interactPromptText;

    int gameGoalCount;
    int gameKillCount;
    int gameDeathCount;
    int gameScore;
    int keyCount;

    public GameObject playerDamageScreen;
    public GameObject playerHealthScreen;

    public TMP_Text gameGoalMenuStat;
    public TMP_Text scoreMenuStat; // Score stat in the main/settings menu
    public TMP_Text killStats;
    public TMP_Text deathStats;


    // List of XRE Modules collected
    public List<XReModule> collectedModules = new List<XReModule>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        minimapCam.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if collected modules is empty and hide minimap if so
        if (collectedModules.Count == 0)
        {
            minimapCam.gameObject.SetActive(false);
        }
        else
        {
            minimapCam.gameObject.SetActive(true);
        }

    }
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        //gameGoalMenuStat.text = gameGoalCount.ToString("F0"); //this line was making the win menu not show up

        // Debug.Log("Game Goal Count: " + gameGoalCount);
        //if (gameGoalCount <= 0 && menuActive == null)
        //{
        //    statePause();
        //    menuActive = menuWin;
        //    // gameScore += 500;
        //    menuActive.SetActive(true);
        //    // soundManager.instance.playVictorySound();
        //}
    }

    public void updateGameKillStat(int amount)
    {
        gameKillCount += amount;
        killStats.text = gameKillCount.ToString("F0");
        Debug.Log("Game Kill Count: " + gameKillCount);
    }

    public void updatePlayerScore(int amount)
    {
        gameScore += amount;
        levelHUDText.text = gameScore.ToString("F0");
        scoreMenuStat.text = gameScore.ToString("F0");
        StartCoroutine(addScoreHUD(amount));
    }
    public void updatePlayerAmmo(int curr, int max)
    {
        playerAmmo.text = curr.ToString("F0") + " / " + max.ToString("F0");

        // Tie amount to Ammo Bar fill amount
        playerAmmoBar.fillAmount = (float)curr / (float)max;
        playerBulletImage.fillAmount = (float)curr / (float)max;
    }
    IEnumerator addScoreHUD(int amount)
    {
        levelHUDFeedbackText.text = amount.ToString("F0");
        Color colorOrig = levelHUDText.color;
        Color colorChange = Color.green;

        levelHUDFeedback.SetActive(true);

        float duration = 0.7f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 1.5f;
        Vector3 endScale = Vector3.zero;

        CanvasGroup cg = levelHUDFeedback.GetComponent<CanvasGroup>();
        cg.alpha = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            levelHUDText.color = Color.Lerp(colorOrig, colorChange, t);
            levelHUDFeedback.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            cg.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        levelHUDText.color = colorOrig;
        levelHUDFeedback.transform.localScale = endScale;
        cg.alpha = 0f;
        levelHUDFeedback.SetActive(false);
        cg.alpha = 1f; // reset alpha for next time
        levelHUDFeedback.transform.localScale = startScale;

    }

    public void UpdateInteractPrompt(string prompt)
    {
        interactPromptText.text = prompt;
    }

    public void UpdateKeyCountText()
    {
        keyCount++;
        keyCountText.text = keyCount.ToString() + "/4";

    }
    public void ActivateAmmoUI()
    {
        playerAmmoCanvas.SetActive(true);
        reticalImage.enabled = true;
    }

    public void DeactivateAmmoUI()
    {
        playerAmmoCanvas.SetActive(false);
        reticalImage.enabled = false;

    }
    public void ActivateModule(XReModule moduleData)
    {
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
                UpdateInteractPrompt(moduleData.activationMessage);

            if (moduleData.activationSound != null)
                AudioSource.PlayClipAtPoint(moduleData.activationSound, transform.position);

        }
        else if (!collectedModules.Contains(moduleData))
        {
            collectedModules.Add(moduleData);

            foreach (string layer in moduleData.layersToEnable)
            {
                // Enable the specified layer in the minimap camera's culling mask
                minimapCam.cullingMask &= ~LayerMask.GetMask(layer); // Unset the bit to show the layer

                // moduleData.Camera.minimapCam.cullingMask |= LayerMask.GetMask(layer);
            }

            foreach (GameObject overlay in moduleData.overlayObjects)
            {
                if (overlay != null) overlay.SetActive(true);
            }

            if (!string.IsNullOrEmpty(moduleData.activationMessage))
                Debug.Log(moduleData.activationMessage);

            if (moduleData.activationSound != null)
                AudioSource.PlayClipAtPoint(moduleData.activationSound, transform.position);
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

    public void LevelUp()
    {
        levelHUDFeedbackText.text = "Level Up!";
        levelHUDFeedback.SetActive(true);
        StartCoroutine(LevelUpRoutine());
        playerLevel++;
        levelHUDText.text = playerLevel.ToString();
    }

    IEnumerator LevelUpRoutine()
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 1.5f;
        Vector3 endScale = Vector3.zero;
        CanvasGroup cg = levelHUDFeedback.GetComponent<CanvasGroup>();
        cg.alpha = 1f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            levelHUDFeedback.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            cg.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        levelHUDFeedback.transform.localScale = endScale;
        cg.alpha = 0f;
        levelHUDFeedback.SetActive(false);
        cg.alpha = 1f; // reset alpha for next time
        levelHUDFeedback.transform.localScale = startScale;
    }
}