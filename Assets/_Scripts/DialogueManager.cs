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

    [Header("References")]
    [SerializeField] private InputActionReference _continueInput;
    
    [SerializeField] private List<CharacterData> _spawnableRobots;
    [SerializeField] private GameObject _robotPre;
    [SerializeField] private GameObject _robotsParent;
    [SerializeField] private Transform _seat0Pos;
    [SerializeField] private Transform _seat1Pos;

    [SerializeField] private TextMeshProUGUI _testTextOutput0;
    [SerializeField] private TextMeshProUGUI _testTextOutput1;

    //Local Variables
    private MatchBalanceSettings BalanceSettings => DayManager.Instance.BalanceSettings;
    private int CurrentCoins => DayManager.Instance.Coins;
    private int currentCompatibilityStrength;
    private bool isMatching;

    private bool seat0Taken;
    private bool seat1Taken;

    private int rejectedCount;
    private int quitCount;
    private int matchedCount;
    private int robotsLeft;




    void Update()
    {
        if (_continueInput.action.WasPressedThisFrame()) PressedContinue();
    }

    public void PressedContinue()
    {
        if (isMatching && DayManager.Instance.DayStarted) CallNextDialogue();
        else if (!isMatching && DayManager.Instance.DayStarted) StartNewDate();
    }

    public void StartDaySetup(int initialRobotCount)
    {
        robotsLeft = initialRobotCount;
        GameUIManager.Instance.DisplayRobotsLeftText(robotsLeft);

        StartNewDate();
    }

    public void StartNewDate()
    {
        if (CanMakeNewPair() == false)
        {
            RobotsFinished();
            return;
        } 

        TrySpawnRobotInEmptySeat(_spawnableRobots[Random.Range(0, _spawnableRobots.Count - 1)]);
        TrySpawnRobotInEmptySeat(_spawnableRobots[Random.Range(0, _spawnableRobots.Count - 1)]);

        InitializeRobots();
        isMatching = true;
    }

    public void RobotsFinished()
    {
        if (seat0Taken) RemoveRobotFromSeat(0);
        if (seat1Taken) RemoveRobotFromSeat(1);

        DayManager.Instance.EndDay(matchedCount);
        
        rejectedCount = 0;
        quitCount = 0;
        matchedCount = 0;
        robotsLeft = 0;

        GameUIManager.Instance.DisplayRejectedCountText(rejectedCount);
        GameUIManager.Instance.DisplayQuitCountText(quitCount);
        GameUIManager.Instance.DisplayRobotsLeftText(robotsLeft);
        GameUIManager.Instance.DisplayRobotValuesTextForSeat(0, "");
        GameUIManager.Instance.DisplayRobotValuesTextForSeat(1, "");
    }

    public void MatchRobotPairing() //Called By Button
    {
        if (ActiveRobotsDict.TryGetValue(0, out Robot robotInSeat0) == false) return;
        if (ActiveRobotsDict.TryGetValue(1, out Robot robotInSeat1) == false) return;

        RemoveRobotFromSeat(0);
        RemoveRobotFromSeat(1);

        DayManager.Instance.UpdateCoins(BalanceSettings.PositiveMatchCoinGain);

        matchedCount++;
        GameUIManager.Instance.DisplayMatchedAndQuotaCountText(matchedCount, DayManager.Instance.MatchedQuota);
    }

    public void RejectRobotInSeat(int seatNum)
    {
        if (ActiveRobotsDict.TryGetValue(seatNum, out Robot robotInSeat) == false) return;
        RemoveRobotFromSeat(seatNum);

        rejectedCount++;
        GameUIManager.Instance.DisplayRejectedCountText(rejectedCount);

        DayManager.Instance.UpdateCoins(BalanceSettings.RobotRejectCoinCost);
    }

    public void RobotQuitInSeat(int seatNum)
    {
        if (CurrentCoins < BalanceSettings.RobotQuitCoinCost) return;

        RemoveRobotFromSeat(seatNum);

        quitCount++;
        GameUIManager.Instance.DisplayQuitCountText(quitCount);

        DayManager.Instance.UpdateCoins(BalanceSettings.RobotQuitCoinCost);
    }

    private void RemoveRobotFromSeat(int seatNum)
    {
        if (ActiveRobotsDict.TryGetValue(seatNum, out Robot robotInSeat) == false) return;

        ActiveRobotsDict.Remove(seatNum);
        robotInSeat.DestroyRobot();

        if (seatNum == 0) seat0Taken = false;
        else if (seatNum == 1) seat1Taken = false;

        isMatching = false;

        GameUIManager.Instance.DisplayRobotValuesTextForSeat(seatNum, ""); 
    }
    private bool CanMakeNewPair()
    {
        if (robotsLeft >= 2) return true;
        if ((robotsLeft == 1) && (ActiveRobotsDict.Count != 0)) return true;

        return false;
    }


    private void TrySpawnRobotInEmptySeat(CharacterData robotCharacterData)
    {
        if (!seat0Taken)
        {
            GameObject robot = Instantiate(_robotPre, _seat0Pos.position, Quaternion.identity, _robotsParent.transform);
            robot.GetComponent<Robot>().InitializeRobot(robotCharacterData, 0);
            seat0Taken = true;

            ActiveRobotsDict.Add(0, robot.GetComponent<Robot>());

            robotsLeft--;
            GameUIManager.Instance.DisplayRobotsLeftText(robotsLeft);
        } 
        else if (!seat1Taken)
        {
            GameObject robot = Instantiate(_robotPre, _seat1Pos.position, Quaternion.identity, _robotsParent.transform);
            robot.GetComponent<Robot>().InitializeRobot(robotCharacterData, 1);
            seat1Taken = true;

            ActiveRobotsDict.Add(1, robot.GetComponent<Robot>());

            robotsLeft--;
            GameUIManager.Instance.DisplayRobotsLeftText(robotsLeft);
        }
    }

    private void InitializeRobots()
    {
        currentCompatibilityStrength = GetCompatibility(ActiveRobotsDict[0].CharacterData.Personality, ActiveRobotsDict[1].CharacterData.Personality);

        int variedCompatibility = currentCompatibilityStrength + VariabilityFunction(0, BalanceSettings.AttractionVariability);
        ActiveRobotsDict[0].ReceiveMatchedRobotCompatibility(variedCompatibility);

        variedCompatibility = currentCompatibilityStrength + VariabilityFunction(0, BalanceSettings.AttractionVariability);
        ActiveRobotsDict[1].ReceiveMatchedRobotCompatibility(variedCompatibility);
    }

    public void CallNextDialogue()
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
    public int GetCompatibility(Personalites a, Personalites b)
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