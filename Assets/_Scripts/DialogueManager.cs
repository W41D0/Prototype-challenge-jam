using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Linq;

public class DialogueManager : Singleton<DialogueManager>
{
    public List<Robot> ActiveRobots;
    public Dictionary<(Personalites, Personalites), int> PersonalityCompatibilityStregthDict = new();
    [SerializeField] private InputActionReference _continueInput;
    
    [SerializeField] private int _textCountBase;
    [SerializeField] private int _textCountVariability;
    [SerializeField] private int _connectionVariability;

    private int currentCompatibilityStrength;


    void Start()
    {
        InitializeRobots();
    }

    private void InitializeRobots()
    {
        currentCompatibilityStrength = GetCompatibility(ActiveRobots[0].CharacterData.Personality, ActiveRobots[1].CharacterData.Personality);

        ActiveRobots[0].ReceiveMatchedRobotCompatibility(currentCompatibilityStrength);
        ActiveRobots[1].ReceiveMatchedRobotCompatibility(currentCompatibilityStrength);
    }

    void Update()
    {
        if (_continueInput.action.WasPressedThisFrame()) CallNextDialogue();
    }
    private void CallNextDialogue()
    {
        if (ActiveRobots.Count != 2) return;

        int speakingBotIndex = RollConfidenceForOrder();
        int listeningBotIndex = (speakingBotIndex == 0) ? 1 : 0;

        int confidenceValue = ActiveRobots[speakingBotIndex].CharacterData.Confidence;
        int textCount = VariabilityFunction(_textCountBase, confidenceValue, _textCountVariability);

        ActiveRobots[speakingBotIndex].SayDialogue(textCount);

        int connectionChangeValue = currentCompatibilityStrength + VariabilityFunction(0,0, _connectionVariability);
        ActiveRobots[listeningBotIndex].ReceiveConvoConnectionValueChange(connectionChangeValue);
    }

    private int RollConfidenceForOrder()
    {
        int robot0Confidence = ActiveRobots[0].CharacterData.Confidence;
        int robot1Confidence = ActiveRobots[1].CharacterData.Confidence;

        int confidenceBias = robot1Confidence - robot0Confidence;
        float confidenceOdds = Mathf.InverseLerp(-10f, 10f, confidenceBias);

        if (Random.value < confidenceOdds) return 1;
        return 0;
    }

    public void AddActiveRobot(Robot robotToAdd)
    {
        if (ActiveRobots.Count >= 2) return;
        ActiveRobots.Add(robotToAdd);
    }

    private int VariabilityFunction(int baseValue, int addedValue, int variability)
    {
        return baseValue + addedValue + Random.Range(-variability, variability);
    }

    public void RemoveActiveRobot(Robot robotToRemove)
    {
        if (ActiveRobots.Count == 0) return;
        ActiveRobots.Remove(robotToRemove);
    }

    public int GetCompatibility(Personalites a, Personalites b)
    {
        int value = 0;

        PersonalityCompatibilityStregthDict.TryGetValue((a, b), out value);
        PersonalityCompatibilityStregthDict.TryGetValue((b, a), out value);

        return value;
    }
}