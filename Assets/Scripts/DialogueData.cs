using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Speaker Info")]
    public string   speakerName;
    public Sprite   speakerPortrait;

    [Header("Dialogue Lines")]
    [TextArea] public string[] dialogueLines;

    [Header("Auto-Progress Settings")]
    public bool  [] autoProgressLines;
    public float    typingSpeed       = 0.05f;
    public float    autoProgressDelay = 2f;
}