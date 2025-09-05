using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }

    public void respawn()
    {
        //gameManager.instance.playerScript.spawnPlayer();
        gameManager.instance.stateUnpause();
    }

    public void openSettings()
    {
        gameManager.instance.openSettings();
    }

    public void openControllerSettings()
    {
        gameManager.instance.openControllerSettings();
    }
    public void openAudioSettings()
    {
        gameManager.instance.openAudioSettings();
    }
}
