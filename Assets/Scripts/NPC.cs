using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{

    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public Text dialogueText, nameText;
    public Image portraitImage;
    public CanvasGroup panelGroup;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    // void Start()
    // {
    //     dialoguePanel.Hide(panelGroup);
    // }
    
    public void Interact()
    {
        if (dialogueData == null)
            return;

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }
    
    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        
        nameText.text = dialogueData.npcName;
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogueData.dialogueLines[dialogueIndex];
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }
        
        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.text = "";
        dialoguePanel.SetActive(false);
    }
    
}
