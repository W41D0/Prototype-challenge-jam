using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DialogueManager : Singleton<DialogueManager>
{
    public List<Robot> ActiveRobots;
    
    [Header("Testing")]
    [SerializeField] private Dialogue[] _testDialogueLines;
    [SerializeField] private float _testTimeBetweenLines;


    void Start()
    {
        StartCoroutine(TestDialogue());
    }


    private IEnumerator TestDialogue()
    {
        foreach (Dialogue dialogueLine in _testDialogueLines)
        {
            Debug.Log($"{dialogueLine.SpeakerName.ToString()}: {dialogueLine.Line}");
            CallDialogue(dialogueLine);
            yield return new WaitForSeconds(_testTimeBetweenLines);
        }
    }  

    private void CallDialogue(Dialogue dialogue)
    {
        Robot speakingRobot = null;

        foreach (Robot robot in ActiveRobots)
        {
            if (robot.CharacterData.Name == dialogue.SpeakerName) speakingRobot = robot;
        }

        if (speakingRobot == null) return;

        Debug.Log($"calling dialogue for: {speakingRobot.name}");
        speakingRobot.SayDialogue(dialogue.Line);
    }

    public void AddActiveRobot(Robot robotToAdd)
    {
        if (ActiveRobots.Count >= 2) return;
        ActiveRobots.Add(robotToAdd);
    }

    public void RemoveActiveRobot(Robot robotToRemove)
    {
        if (ActiveRobots.Count == 0) return;
        ActiveRobots.Remove(robotToRemove);
    }
}


[System.Serializable]
public class Dialogue
{
    public Names SpeakerName;
    public string Line;

}