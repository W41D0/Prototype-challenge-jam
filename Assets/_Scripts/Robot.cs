using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using NUnit.Framework;

public class Robot : MonoBehaviour
{
    [Header("Personality")]
    [field: SerializeField] public CharacterData CharacterData {get; private set;}
    public float Interest {get; private set;} // declines slowly across convo if it reaches zero robot leaves (how long you're talking) /10
    public int Connection {get; private set;} // changes with the how the conversation is going (what youre talking about) /10
    public int Attraction {get; private set;} // once they meet first time (initial vibes) /10

    public bool IsVibing {get; private set;}
    public bool LostPatience {get; private set;}

    [Header("References")]
    [SerializeField] private Transform _textBoxPos;
    [SerializeField] private GameObject _dialogueBubblePrefab;





    private int linesReceivedCount;
    private int linesSentCount;
    private int seatNumber;
    private GameObject currentActiveTextBubble;

    private MatchBalanceSettings BalanceSettings => DayManager.Instance.BalanceSettings;


    public void InitializeRobot(CharacterData characterData, int seatNum)
    {
        CharacterData = characterData;
        seatNumber = seatNum;
    }

    public void DestroyRobot()
    {
        Destroy(gameObject);
    } 

    public void ReceiveMatchedRobotCompatibility(int compatibilityStrength)
    {
        Attraction = compatibilityStrength + VariabilityFunction(0, BalanceSettings.AttractionBalanceVariability);
        if (Attraction > BalanceSettings.MaxAttraction) Attraction = BalanceSettings.MaxAttraction;
        if (Attraction < 0) Attraction = 0;

        InitialInterest();
        ShowValues(); 
    }

    private void InitialInterest()
    {
        int initialInterestReduction = Mathf.RoundToInt((10 - Attraction) / 10f * BalanceSettings.MaxInitialInterestReduction);

        Interest = 10 - initialInterestReduction;

        if (Interest > BalanceSettings.MaxInterest) Interest = BalanceSettings.MaxInterest;
        if (Interest < 0) Interest = 0;
    }

    public void ReceiveConvoConnectionValueChange(int connectionChange)
    {
        Connection += connectionChange;
        if (Connection > BalanceSettings.MaxConnection) Connection = BalanceSettings.MaxConnection;
        if (Connection < 0) Connection = 0;

        linesReceivedCount++;
        CalculateInterest();

        if (Interest == 0) DialogueManager.Instance.RobotQuitInSeat(seatNumber);

        ShowValues();
    }

    private void CalculateInterest()
    {
        int conversationLength = linesReceivedCount + linesSentCount;

        IsVibing = Connection > CharacterData.Pickiness;
        LostPatience = ((conversationLength * BalanceSettings.ConversationLengthBalanceMultiplier) > CharacterData.Patience) && (IsVibing == false);

        if (IsVibing)
        {
            Interest -= BalanceSettings.IsVibingInterestDecrement;
        }
        else if (LostPatience)
        {
            Interest -= BalanceSettings.LostPatienceInterestDecrement;
        }
        else
        {
            Interest -= BalanceSettings.HasPatienceInterestDecrement;
        }

        if (Interest > BalanceSettings.MaxInterest) Interest = BalanceSettings.MaxInterest;
        if (Interest < 0) Interest = 0;
    }

    public void SayDialogue(int textCount)
    {
        if (currentActiveTextBubble != null) DeleteActiveTextBubble();

        GameObject textBubble = Instantiate(_dialogueBubblePrefab, _textBoxPos.position, Quaternion.identity, _textBoxPos);
        textBubble.GetComponent<DialogueBox>().DisplayText(textCount);

        currentActiveTextBubble = textBubble;

        linesSentCount++;
    }

    private int VariabilityFunction(int baseValue, int variability)
    {
        return baseValue + Random.Range(-variability, variability);
    }

    private void DeleteActiveTextBubble()
    {
        currentActiveTextBubble.GetComponent<DialogueBox>().DeleteBubble();
        currentActiveTextBubble = null;
    }

    private void ShowValues()
    {
       string text = $"Interest: {Interest}\nConnection: {Connection}\nAttraction: {Attraction}\nLines Sent: {linesSentCount}\nLines Received: {linesReceivedCount}\nIsVibing: {IsVibing}\nLostPatience: {LostPatience}";
        GameUIManager.Instance.DisplayRobotValuesTextForSeat(seatNumber, text);
    }
}
