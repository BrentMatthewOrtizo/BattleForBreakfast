using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerDialogue", menuName = "Player Dialogue")]
public class PlayerDialogue : ScriptableObject
{
    public string playerName;
    public Sprite playerPortrait;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public float typingSpeed = 0.05f;
    public float autoProgressDelay = 2f;
}
