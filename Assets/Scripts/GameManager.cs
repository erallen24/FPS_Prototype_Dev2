using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPrev;
    [SerializeField] GameObject subMenuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuSettings;

    [SerializeField] GameObject subMenuGameplay;
    [SerializeField] GameObject subMenuControls;
    [SerializeField] GameObject subMenuAudio;

    [SerializeField] GameObject warningHUD;
    [SerializeField] TMP_Text warningHUDLabel;
    [SerializeField] GameObject warningHUDTextBox;
    [SerializeField] TMP_Text warningHUDText;

    [SerializeField] TMP_Text scoreHUDText;
    [SerializeField] GameObject scoreHUDFeedback;
    [SerializeField] TMP_Text scoreHUDFeedbackText;


    [SerializeField] TMP_Text playerAmmo;
    [SerializeField] TMP_Text gameGoalCountText;






    public Transform playerRetical;
    public Image playerHPBar;
    // public Image playerRageBar;
    public Image playerStaminaBar;
    public GameObject playerAmmoCanvas;
    public Image playerAmmoBar;
    public Image playerBulletImage;

    public GameObject playerDamageScreen;
    public GameObject playerHealthScreen;
    // public GameObject playerRageScreen;


    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawnPos;
    public GameObject checkpointPopup;

    public TMP_Text gameGoalMenuStat;
    public TMP_Text scoreMenuStat; // Score stat in the main/settings menu
    public TMP_Text killStats;
    public TMP_Text deathStats;



    int gameGoalCount;
    int gameKillCount;
    int gameDeathCount;
    int gameScore;

    public bool isPaused;

    float timeScaleOrig;

    void Start()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");

        playerScript = player.GetComponent<PlayerController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

    }


    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }

            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }

    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        transform.GetComponent<AudioSource>().Pause();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        transform.GetComponent<AudioSource>().Play();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (subMenuActive != null)
            subMenuActive.SetActive(false);
        if (menuMain != null)
            menuMain.SetActive(false);

        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        //gameGoalMenuStat.text = gameGoalCount.ToString("F0"); //this line was making the win menu not show up

        //Debug.Log("Game Goal Count: " + gameGoalCount);
        if (gameGoalCount <= 0/* && menuActive == null*/)
        {
            statePause();
            menuActive = menuWin;
            // gameScore += 500;
            menuActive.SetActive(true);
            // soundManager.instance.playVictorySound();
        }
    }

    public void youLose()
    {
        if (menuActive == null)
        {
            gameDeathCount++;
            //deathStats.text = gameDeathCount.ToString("F0");
            statePause();
            //soundManager.instance.playDeathSound();
            menuActive = menuLose;
            menuActive.SetActive(true);
            //soundManager.instance.playLoseSound();

        }
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
        scoreHUDText.text = gameScore.ToString("F0");
        scoreMenuStat.text = gameScore.ToString("F0");
        StartCoroutine(addScoreHUD(amount));
    }

    public void openSettings()
    {
        if (subMenuActive != null && subMenuActive != subMenuGameplay)
            subMenuActive.SetActive(false);

        if (menuActive != null && menuActive != menuSettings)
            menuActive.SetActive(false);

        menuPrev = menuActive;
        menuActive = menuSettings;
        subMenuActive = subMenuGameplay;
        menuMain.SetActive(true);
        subMenuGameplay.SetActive(true);
        menuActive.SetActive(true);

    }
    public void closeSettings()
    {
        menuActive = menuPrev;
        menuMain.SetActive(false);
        subMenuActive.SetActive(false);
        menuActive.SetActive(true);
    }

    public void updatePlayerAmmo(int curr, int max)
    {
        playerAmmo.text = curr.ToString("F0") + " / " + max.ToString("F0");

        // Tie amount to Ammo Bar fill amount
        playerAmmoBar.fillAmount = (float)curr / (float)max;
        playerBulletImage.fillAmount = (float)curr / (float)max;
    }
    public void openControllerSettings()
    {
        subMenuActive.SetActive(false);
        subMenuActive = subMenuControls;
        subMenuActive.SetActive(true);
    }

    public void openAudioSettings()
    {
        subMenuActive.SetActive(false);
        subMenuActive = subMenuAudio;
        subMenuActive.SetActive(true);
    }

    IEnumerator addScoreHUD(int amount)
    {
        scoreHUDFeedbackText.text = amount.ToString("F0");
        Color colorOrig = scoreHUDText.color;
        Color colorChange = Color.green;

        scoreHUDFeedback.SetActive(true);

        float duration = 0.7f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 1.5f;
        Vector3 endScale = Vector3.zero;

        CanvasGroup cg = scoreHUDFeedback.GetComponent<CanvasGroup>();
        cg.alpha = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            scoreHUDText.color = Color.Lerp(colorOrig, colorChange, t);
            scoreHUDFeedback.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            cg.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        scoreHUDText.color = colorOrig;
        scoreHUDFeedback.transform.localScale = endScale;
        cg.alpha = 0f;
        scoreHUDFeedback.SetActive(false);
        cg.alpha = 1f; // reset alpha for next time
        scoreHUDFeedback.transform.localScale = startScale;

    }

    // Create coroutine for showing and masking warnings
    public IEnumerator showWarning(string label, string text, float duration)
    {
        Transform transformOrig = warningHUD.transform;
        Transform textBoxTransOrig = warningHUDTextBox.transform;

        warningHUDLabel.text = label;
        warningHUDText.text = text;
        warningHUD.SetActive(true);
        // Lerp in the warning box from the original y position to a difference of -39 on the Y axis
        float elapsed = 0f;
        float lerpDuration = 0.5f;
        Vector3 endPos = new Vector3(transformOrig.position.x, transformOrig.position.y - 39, transformOrig.position.z);
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            warningHUD.transform.position = Vector3.Lerp(transformOrig.position, endPos, t);
            yield return null;
        }
        // Lerp the text box in from the original x position to a difference of -39 on the Y axis
        elapsed = 0f;
        Vector3 textBoxEndPos = new Vector3(textBoxTransOrig.position.x, textBoxTransOrig.position.y - 128, textBoxTransOrig.position.z);
        warningHUDTextBox.SetActive(true);
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            warningHUDTextBox.transform.position = Vector3.Lerp(textBoxTransOrig.position, textBoxEndPos, t);
            yield return null;
        }


        yield return new WaitForSeconds(duration);

        // Lerp out the text box back to the original position
        elapsed = 0f;
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            warningHUDTextBox.transform.position = Vector3.Lerp(textBoxEndPos, textBoxTransOrig.position, t);
            yield return null;
        }
        warningHUDTextBox.SetActive(false);

        // Lerp out the warning box back to the original position
        elapsed = 0f;
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            warningHUD.transform.position = Vector3.Lerp(endPos, transformOrig.position, t);
            yield return null;
        }
        warningHUD.SetActive(false);
    }


}
