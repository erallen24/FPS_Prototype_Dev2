using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    public static InfoManager instance;

    [SerializeField] GameObject infoLabelWindow;
    [SerializeField] TMP_Text infoLabelText;
    [SerializeField] GameObject infoMessageWindow;
    [SerializeField] TMP_Text infoMessageText;
    [SerializeField] Image line;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Create coroutine for showing and masking warnings
    public IEnumerator showWarning(string label, string text, float duration)
    {
        Transform transformOrig = infoLabelWindow.transform;
        Transform textBoxTransOrig = infoMessageWindow.transform;

        infoLabelText.text = label;
        infoMessageText.text = text;
        infoLabelWindow.SetActive(true);
        // Lerp in the warning box from the original y position to a difference of -39 on the Y axis
        float elapsed = 0f;
        float lerpDuration = 0.5f;
        Vector3 endPos = new Vector3(transformOrig.position.x, transformOrig.position.y - 39, transformOrig.position.z);
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            infoLabelWindow.transform.position = Vector3.Lerp(transformOrig.position, endPos, t);
            yield return null;
        }
        // Lerp the text box in from the original x position to a difference of -39 on the Y axis
        elapsed = 0f;
        Vector3 textBoxEndPos = new Vector3(textBoxTransOrig.position.x, textBoxTransOrig.position.y - 128, textBoxTransOrig.position.z);
        infoMessageWindow.SetActive(true);
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            infoMessageWindow.transform.position = Vector3.Lerp(textBoxTransOrig.position, textBoxEndPos, t);
            yield return null;
        }


        yield return new WaitForSeconds(duration);

        // Lerp out the text box back to the original position
        elapsed = 0f;
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            infoMessageWindow.transform.position = Vector3.Lerp(textBoxEndPos, textBoxTransOrig.position, t);
            yield return null;
        }
        infoMessageWindow.SetActive(false);

        // Lerp out the warning box back to the original position
        elapsed = 0f;
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            infoLabelWindow.transform.position = Vector3.Lerp(endPos, transformOrig.position, t);
            yield return null;
        }
        infoLabelWindow.SetActive(false);
    }
}
