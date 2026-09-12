using System.Collections.Generic;
using UnityEngine;
 
 
[System.Serializable]
public class DialogueLine
{
    public CharacterData character;
    [TextArea(3, 10)]
    public string line;
}
 
[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}
 
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
 
    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    void Start()
    {
        TriggerDialogue();
    }
}

