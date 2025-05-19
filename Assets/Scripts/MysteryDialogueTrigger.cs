using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MysteryDialogueTrigger : MonoBehaviour, IInteractable
{
    
    public SpriteRenderer targetSprite;

    [Header("← Dialogue Data & UI")]
    public DialogueData data;
    public GameObject   dialoguePanel;
    public Text         nameText;
    public Text         bodyText;
    public Image        portraitImage;
    
    public Behaviour[]  movementScripts;
    
    public bool  startOnAwake = false;
    public float startDelay   = 0f;

    int   currentLine;
    bool  isTyping, isActive, hasRevealed;

    void Start()
    {
        if (targetSprite != null)
            SetAlpha(0);
        
        dialoguePanel.SetActive(false);
        
        if (startOnAwake)
            StartCoroutine(AutoStart());
    }

    IEnumerator AutoStart()
    {
        yield return new WaitForSeconds(startDelay);
        BeginDialogue();
    }

    public bool CanInteract()
    {
        if (isTyping || startOnAwake) 
            return false;
        
        if (targetSprite != null)
        {
            if (!SpawnManager.hasCereal || hasRevealed)
                return false;
        }

        return true;
    }

    public void Interact()
    {
        if (!hasRevealed)
        {
            hasRevealed = true;
            SetAlpha(1);
        }
        
        if (!isActive)           BeginDialogue();
        else if (isTyping)       SkipTyping();
        else                     NextLine();
    }

    void SetAlpha(float a)
    {
        var c = targetSprite.color;
        c.a = a;
        targetSprite.color = c;
    }

    void BeginDialogue()
    {
        if (data == null) return;

        isActive    = true;
        currentLine = 0;
        dialoguePanel.SetActive(true);

        nameText.text        = data.speakerName;
        portraitImage.sprite = data.speakerPortrait;

        ToggleMovement(false);
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping       = true;
        bodyText.text  = "";

        string line = data.dialogueLines[currentLine];
        foreach (char ch in line)
        {
            bodyText.text += ch;
            yield return new WaitForSeconds(data.typingSpeed);
        }

        isTyping = false;
        
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
        isTyping      = false;
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