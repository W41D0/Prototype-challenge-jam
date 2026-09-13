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
    public Dictionary<int, Robot> ActiveRobotsDict = new ();
    public List<PairingCompatibility> PersonalityCompatibilityStregthList = new();
    [SerializeField] private InputActionReference _continueInput;
    
    [SerializeField] private List<CharacterData> _spawnableRobots;
    [SerializeField] private GameObject _robotPre;
    [SerializeField] private GameObject _robotsParent;
    [SerializeField] private Transform _seat0Pos;
    [SerializeField] private Transform _seat1Pos;

    [SerializeField] private TextMeshProUGUI _testTextOutput0;
    [SerializeField] private TextMeshProUGUI _testTextOutput1;

    [Header("Balance Settings")]
    public MatchBalanceSettings BalanceSettings;
 
    private int currentCompatibilityStrength;
    private bool startedGame;
    private bool isMatching;

    private bool seat0Taken;
    private bool seat1Taken;

    private int rejectedCount;
    private int matchedCount;

    void Start()
    {
        
    }
    void Update()
    {
        bool pressedContinue = _continueInput.action.WasPressedThisFrame();

        if (pressedContinue && isMatching && startedGame) CallNextDialogue();
        else if (pressedContinue && !isMatching && startedGame) StartGame();
        else if (pressedContinue && !isMatching && !startedGame) StartGame();
    }

    public void StartGame()
    {
        SpawnRobotInEmptySeat(_spawnableRobots[Random.Range(0, _spawnableRobots.Count - 1)]);
        SpawnRobotInEmptySeat(_spawnableRobots[Random.Range(0, _spawnableRobots.Count - 1)]);
        
        startedGame = true;
        StartDate();
    }

    public void StartDate()
    {
        InitializeRobots();
        isMatching = true;
    }

    public void MatchRobotPairing()
    {
        if (ActiveRobotsDict.TryGetValue(0, out Robot robotInSeat0) == false) return;
        if (ActiveRobotsDict.TryGetValue(1, out Robot robotInSeat1) == false) return;

        robotInSeat0.DestroyRobot();
        RemoveActiveRobot(0);
        robotInSeat1.DestroyRobot();
        RemoveActiveRobot(1);

        matchedCount++;
        GameUIManager.Instance.DisplayMatchedCount(matchedCount);
    }

    public void RejectRobotInSeat(int seatNum)
    {
        if (ActiveRobotsDict.TryGetValue(seatNum, out Robot robotInSeat) == false) return; 

        robotInSeat.DestroyRobot();
        RemoveActiveRobot(seatNum);

        rejectedCount++;
        GameUIManager.Instance.DisplayRejectedCount(rejectedCount);
    }

    private void SpawnRobotInEmptySeat(CharacterData robotCharacterData)
    {
        if (!seat0Taken)
        {
            GameObject robot = Instantiate(_robotPre, _seat0Pos.position, Quaternion.identity, _robotsParent.transform);
            robot.GetComponent<Robot>().InitializeRobot(robotCharacterData, 0);
            seat0Taken = true;

            ActiveRobotsDict.Add(0, robot.GetComponent<Robot>());
        } 
        else if (!seat1Taken)
        {
            GameObject robot = Instantiate(_robotPre, _seat1Pos.position, Quaternion.identity, _robotsParent.transform);
            robot.GetComponent<Robot>().InitializeRobot(robotCharacterData, 1);
            seat1Taken = true;

            ActiveRobotsDict.Add(1, robot.GetComponent<Robot>());
        }
    }

    public void RemoveActiveRobot(int seatNum)
    {
        if (ActiveRobotsDict.Count == 0) return;
        ActiveRobotsDict.Remove(seatNum);

        if (seatNum == 0) seat0Taken = false;
        else if (seatNum == 1) seat1Taken = false;

        isMatching = false;
    }

    private void InitializeRobots()
    {
        currentCompatibilityStrength = GetCompatibility(ActiveRobotsDict[0].CharacterData.Personality, ActiveRobotsDict[1].CharacterData.Personality);

        int variedCompatibility = currentCompatibilityStrength + VariabilityFunction(0, BalanceSettings.AttractionVariability);
        ActiveRobotsDict[0].ReceiveMatchedRobotCompatibility(variedCompatibility);

        variedCompatibility = currentCompatibilityStrength + VariabilityFunction(0, BalanceSettings.AttractionVariability);
        ActiveRobotsDict[1].ReceiveMatchedRobotCompatibility(variedCompatibility);
    }

    private void CallNextDialogue()
    {
        if (ActiveRobotsDict.Count != 2) return;

        int speakingBotIndex = RollConfidenceForOrder();
        int listeningBotIndex = (speakingBotIndex == 0) ? 1 : 0;

        int confidenceValue = ActiveRobotsDict[speakingBotIndex].CharacterData.Confidence;
        int textCount = VariabilityFunction(BalanceSettings.TextCountBase + confidenceValue, BalanceSettings.TextCountVariability);


        ActiveRobotsDict[speakingBotIndex].SayDialogue(textCount);


        int randomRoll = Mathf.RoundToInt(Random.Range(-BalanceSettings.ConnectionRandomRollBaseChange, BalanceSettings.ConnectionRandomRollBaseChange));
        randomRoll += VariabilityFunction(BalanceSettings.ConnectionBaseModifier, BalanceSettings.ConnectionVariability);
        ActiveRobotsDict[listeningBotIndex].ReceiveConvoConnectionValueChange(randomRoll);

        randomRoll = Mathf.RoundToInt(Random.Range(-BalanceSettings.ConnectionRandomRollBaseChange, BalanceSettings.ConnectionRandomRollBaseChange));
        randomRoll += VariabilityFunction(BalanceSettings.ConnectionBaseModifier, BalanceSettings.ConnectionVariability);
        ActiveRobotsDict[speakingBotIndex].ReceiveConvoConnectionValueChange(randomRoll);
    }

    private int RollConfidenceForOrder()
    {
        int robot0Confidence = ActiveRobotsDict[0].CharacterData.Confidence;
        int robot1Confidence = ActiveRobotsDict[1].CharacterData.Confidence;

        int confidenceBias = robot1Confidence - robot0Confidence;
        float confidenceOdds = Mathf.InverseLerp(-10f, 10f, confidenceBias);

        if (Random.value < confidenceOdds) return 1;
        return 0;
    }



    private int VariabilityFunction(int baseValue, int variability)
    {
        return baseValue + Random.Range(-variability, variability);
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

    float RemapRange(float value, Vector2 oldRange, Vector2 newRange)
    {
        return newRange.x + (value - oldRange.x) / (oldRange.y - oldRange.x) * (newRange.y - newRange.x);
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