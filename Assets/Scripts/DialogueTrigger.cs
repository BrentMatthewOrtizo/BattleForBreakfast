using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("Data & UI")]
    public DialogueData data;
    public GameObject   dialoguePanel;
    public Text         nameText;
    public Text         bodyText;
    public Image        portraitImage;

    [Header("Who to pause")]
    [Tooltip("Drag in PlayerMovement, WaypointMover, etc.")]
    public Behaviour[] movementScripts;

    [Header("Auto-Start (e.g. player intro)")]
    public bool  startOnAwake = false;
    public float startDelay   = 0f;

    int   currentLine;
    bool  isTyping;
    bool  isActive;

    void Start()
    {
        dialoguePanel.SetActive(false);
        if (startOnAwake)
            StartCoroutine(StartAfterDelay());
    }

    IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        BeginDialogue();
    }

    public void Interact()
    {
        if (!isActive)           BeginDialogue();
        else if (isTyping)       SkipTyping();
        else                     NextLine();
    }

    public bool CanInteract() => !isTyping && !startOnAwake;

    void BeginDialogue()
    {
        if (data == null) return;

        isActive       = true;
        currentLine    = 0;
        dialoguePanel.SetActive(true);

        nameText.text       = data.speakerName;
        portraitImage.sprite = data.speakerPortrait;

        ToggleMovement(false);
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping   = true;
        bodyText.text = "";

        string line = data.dialogueLines[currentLine];
        foreach (char c in line)
        {
            bodyText.text += c;
            yield return new WaitForSeconds(data.typingSpeed);
        }

        isTyping = false;

        // auto-advance?
        if (currentLine < data.autoProgressLines.Length 
            && data.autoProgressLines[currentLine])
        {
            yield return new WaitForSeconds(data.autoProgressDelay);
            NextLine();
        }
    }

    void SkipTyping()
    {
        StopAllCoroutines();
        isTyping    = false;
        bodyText.text = data.dialogueLines[currentLine];
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < data.dialogueLines.Length)
            StartCoroutine(TypeLine());
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        dialoguePanel.SetActive(false);
        isActive = isTyping = false;
        ToggleMovement(true);
    }

    void ToggleMovement(bool on)
    {
        foreach (var comp in movementScripts)
            comp.enabled = on;
    }
}