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
    public List<PairingCompatibility> PersonalityCompatibilityStregthList = new();
    [SerializeField] private InputActionReference _continueInput;
    
    [SerializeField] private int _textCountBase;
    [SerializeField] private int _textCountVariability;
    [SerializeField] private int _connectionBaseModifier;
    [SerializeField] private int _connectionVariability;
    [SerializeField] private int _attractionVariability;

    private int currentCompatibilityStrength;


    void Start()
    {
        InitializeRobots();
    }

    private void InitializeRobots()
    {
        currentCompatibilityStrength = GetCompatibility(ActiveRobots[0].CharacterData.Personality, ActiveRobots[1].CharacterData.Personality);

        int variedCompatibility = currentCompatibilityStrength + VariabilityFunction(0,0, _attractionVariability);
        ActiveRobots[0].ReceiveMatchedRobotCompatibility(variedCompatibility);

        variedCompatibility = currentCompatibilityStrength + VariabilityFunction(0,0, _attractionVariability);
        ActiveRobots[1].ReceiveMatchedRobotCompatibility(variedCompatibility);
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


        int variedCompatibility = currentCompatibilityStrength + VariabilityFunction(_connectionBaseModifier,0, _connectionVariability);
        int connectionChangeValue = variedCompatibility * 2 - 10; //0_1 to -10_10
        ActiveRobots[listeningBotIndex].ReceiveConvoConnectionValueChange(connectionChangeValue);

        variedCompatibility = currentCompatibilityStrength + VariabilityFunction(_connectionBaseModifier,0, _connectionVariability);
        connectionChangeValue = variedCompatibility * 2 - 10;
        ActiveRobots[speakingBotIndex].ReceiveConvoConnectionValueChange(connectionChangeValue);
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

    int GetCompatibility(Personalites a, Personalites b)
    {
        foreach (PairingCompatibility pairing in PersonalityCompatibilityStregthList)
        {
            if ((pairing.A == a && pairing.B == b) ||
                (pairing.A == b && pairing.B == a))
            {
                return pairing.Strength;
            }
        }

        return 0;
    }
}


[System.Serializable]
public class PairingCompatibility
{
    public Personalites A;
    public Personalites B;
    [Range(0, 10)]
    public int Strength;
}