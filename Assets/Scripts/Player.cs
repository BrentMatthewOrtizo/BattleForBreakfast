using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

    public class Player : MonoBehaviour, IInteractable
    {

        public PlayerDialogue dialogueData;
        public GameObject dialoguePanel;
        public Text dialogueText, nameText;
        public Image portraitImage;
        public PlayerMovement movement;

        private int dialogueIndex;
        private bool isTyping, isDialogueActive;

        void Start()
        {
            dialoguePanel.SetActive(false);
            if (SceneManager.GetActiveScene().name == "UpstairsHouseScene")
            {
                StartCoroutine(WaitForStartDialogue());
            }
        }

        IEnumerator WaitForStartDialogue()
        {
            yield return new WaitForSeconds(2f);
            StartDialogue();
        }
    
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
            movement.SetMovementEnabled(false);
        
            nameText.text = dialogueData.playerName;
            portraitImage.sprite = dialogueData.playerPortrait;

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
            movement.SetMovementEnabled(true);
            StopAllCoroutines();
            isDialogueActive = false;
            dialogueText.text = "";
            dialoguePanel.SetActive(false);
        }
        // testing
    }