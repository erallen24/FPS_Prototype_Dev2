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

    [SerializeField] bool isShowing = false;
    [SerializeField] float lerpDuration = 0.5f;
    private float elapsed = 0f;
    private float waitTime = 0f;
    private Vector3 labelOrigPos;
    private Vector3 labelHidPos;
    private Vector3 textBoxOrigPos;
    private Vector3 textBoxHidPos;
    private Coroutine currentCoroutine;
    private bool isCoroutineRunning = false;
    private bool isInfoShowing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        infoLabelWindow.SetActive(false);
        infoMessageWindow.SetActive(false);
        labelOrigPos = infoLabelWindow.transform.position;
        labelHidPos = new Vector3(labelOrigPos.x, labelOrigPos.y + 38, labelOrigPos.z);
        textBoxOrigPos = infoMessageWindow.transform.position;
        textBoxHidPos = new Vector3(textBoxOrigPos.x, textBoxOrigPos.y + 130, textBoxOrigPos.z);
        infoLabelWindow.transform.position = labelHidPos;
        infoMessageWindow.transform.position = textBoxHidPos;


    }

    // Update is called once per frame
    void Update()
    {



    }

    // Make an Enumerator to show a warning message titled with a label for a set duration
    public void ShowWarning(string label, string text, int duration = 5)
    {
        if (isCoroutineRunning)
        {
            StopCoroutine(currentCoroutine);
            isCoroutineRunning = false;
        }
        currentCoroutine = StartCoroutine(showWarning(label, text, duration));

    }
    public void HideWarning()
    {
        if (isCoroutineRunning)
        {
            StopCoroutine(currentCoroutine);
            isCoroutineRunning = false;
        }
        currentCoroutine = StartCoroutine(hideWarning());

    }

    public IEnumerator hideWarning()
    {
        isCoroutineRunning = true;
        // Lerp out the text box back to the original position
        elapsed = 0f;

        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            infoMessageWindow.transform.position = Vector3.Lerp(textBoxOrigPos, textBoxHidPos, t);
            yield return null;
        }
        infoMessageWindow.SetActive(false);
        // Lerp out the warning box back to the original position
        elapsed = 0f;

        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            infoLabelWindow.transform.position = Vector3.Lerp(labelOrigPos, labelHidPos, t);
            yield return null;

        }
        infoLabelWindow.SetActive(false);
        isCoroutineRunning = false;
        isInfoShowing = false;
    }

    public IEnumerator showWarning(string label, string text, int duration)
    {
        isCoroutineRunning = true;
        isInfoShowing = true;
        Transform transformOrig = infoLabelWindow.transform;
        infoLabelText.text = label;
        infoMessageText.text = text;
        infoLabelWindow.SetActive(true);
        // Lerp in the warning box from the original y position to a difference of -39 on the Y axis
        elapsed = 0f;

        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            infoLabelWindow.transform.position = Vector3.Lerp(labelHidPos, labelOrigPos, t);
            yield return null;
        }
        // Lerp the text box in from the original x position to a difference of -39 on the Y axis
        elapsed = 0f;

        infoMessageWindow.SetActive(true);
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            infoMessageWindow.transform.position = Vector3.Lerp(textBoxHidPos, textBoxOrigPos, t);
            yield return null;
        }
        yield return new WaitForSeconds(duration);
        isCoroutineRunning = false;
        StartCoroutine(hideWarning());

    }


    //// Create coroutine for showing and masking warnings
    //public IEnumerator showWarning(string label, string text, float duration)
    //{
    //    Transform transformOrig = infoLabelWindow.transform;
    //    Transform textBoxTransOrig = infoMessageWindow.transform;

    //    infoLabelText.text = label;
    //    infoMessageText.text = text;
    //    infoLabelWindow.SetActive(true);
    //    // Lerp in the warning box from the original y position to a difference of -39 on the Y axis
    //    //float elapsed = 0f;
    //    //float lerpDuration = 0.5f;
    //    Vector3 endPos = new Vector3(transformOrig.position.x, transformOrig.position.y - 39, transformOrig.position.z);
    //    while (elapsed < lerpDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / lerpDuration);
    //        infoLabelWindow.transform.position = Vector3.Lerp(transformOrig.position, endPos, t);
    //        yield return null;
    //    }
    //    // Lerp the text box in from the original x position to a difference of -39 on the Y axis
    //    elapsed = 0f;
    //    Vector3 textBoxEndPos = new Vector3(textBoxTransOrig.position.x, textBoxTransOrig.position.y - 128, textBoxTransOrig.position.z);
    //    infoMessageWindow.SetActive(true);
    //    while (elapsed < lerpDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / lerpDuration);
    //        infoMessageWindow.transform.position = Vector3.Lerp(textBoxTransOrig.position, textBoxEndPos, t);
    //        yield return null;
    //    }


    //    yield return new WaitForSeconds(duration);

    //    // Lerp out the text box back to the original position
    //    elapsed = 0f;
    //    while (elapsed < lerpDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / lerpDuration);
    //        infoMessageWindow.transform.position = Vector3.Lerp(textBoxEndPos, textBoxTransOrig.position, t);
    //        yield return null;
    //    }
    //    infoMessageWindow.SetActive(false);

    //    // Lerp out the warning box back to the original position
    //    elapsed = 0f;
    //    while (elapsed < lerpDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / lerpDuration);
    //        infoLabelWindow.transform.position = Vector3.Lerp(endPos, transformOrig.position, t);
    //        yield return null;
    //    }
    //    infoLabelWindow.SetActive(false);
    //}

    //public void ShowUrgent(string label, string text, float duration = 2f)
    //{
    //    if (isCoroutineRunning)
    //    {
    //        StopCoroutine(currentCoroutine);
    //        isCoroutineRunning = false;
    //    }
    //    currentCoroutine = StartCoroutine(showWarning(label, text, duration));
    //    isCoroutineRunning = true;
    //}

    //public void ShowInfo(string text, float duration = 2f)
    //{
    //    if (isTextBoxCoroutineRunning)
    //    {
    //        StopCoroutine(textBoxCoroutine);
    //        isTextBoxCoroutineRunning = false;
    //    }
    //    textBoxCoroutine = StartCoroutine(showTextBox(text, duration));
    //    isTextBoxCoroutineRunning = true;
    //}
    //public IEnumerator showWarning(string label, string text, float duration)
    //{
    //    isInfoShowing = true;
    //    Transform transformOrig = infoLabelWindow.transform;
    //    infoLabelText.text = label;
    //    infoMessageText.text = text;
    //    infoLabelWindow.SetActive(true);
    //    // Lerp in the warning box from the original y position to a difference of -39 on the Y axis
    //    elapsed = 0f;
    //    Vector3 endPos = new Vector3(transformOrig.position.x, transformOrig.position.y - 39, transformOrig.position.z);
    //    while (elapsed < lerpDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / lerpDuration);
    //        infoLabelWindow.transform.position = Vector3.Lerp(transformOrig.position, endPos, t);
    //        yield return null;
    //    }
    //    // Lerp the text box in from the original x position to a difference of -39 on the Y axis
    //    elapsed = 0f;
    //    Vector3 textBoxTransOrig = infoMessageWindow.transform.position;
    //    Vector3 textBoxEndPos = new Vector3(textBoxTransOrig.x, textBoxTransOrig.y - 128, textBoxTransOrig.z);
    //    infoMessageWindow.SetActive(true);
    //    while (elapsed < lerpDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / lerpDuration);
    //        infoMessageWindow.transform.position = Vector3.Lerp(textBoxTransOrig, textBoxEndPos, t);
    //        yield return null;
    //    }
    //    yield return new WaitForSeconds(duration);
    //}



    //public IEnumerator showTextBox(string text, float duration)
    //{
    //    Transform textBoxTransOrig = infoMessageWindow.transform;
    //    infoMessageText.text = text;
    //    infoMessageWindow.SetActive(true);
    //    // Lerp the text box in from the original x position to a difference of -39 on the Y axis
    //    elapsed = 0f;
    //    Vector3 textBoxEndPos = new Vector3(textBoxTransOrig.position.x, textBoxTransOrig.position.y - 128, textBoxTransOrig.position.z);
    //    while (elapsed < lerpDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / lerpDuration);
    //        infoMessageWindow.transform.position = Vector3.Lerp(textBoxTransOrig.position, textBoxEndPos, t);
    //        yield return null;
    //    }
    //    yield return new WaitForSeconds(duration);
    //    // Lerp out the text box back to the original position
    //    elapsed = 0f;
    //    while (elapsed < lerpDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / lerpDuration);
    //        infoMessageWindow.transform.position = Vector3.Lerp(textBoxEndPos, textBoxTransOrig.position, t);
    //        yield return null;
    //    }
    //    infoMessageWindow.SetActive(false);
    //    isTextBoxCoroutineRunning = false;
    //    isInfoShowing = false;
    //}

    public bool IsInfoShowing() { return isInfoShowing; }
}
