using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using NUnit.Framework;

public class Robot : MonoBehaviour
{

    [Header("Personality")]
    [field: SerializeField] public CharacterData CharacterData {get; private set;}
    public int Interest {get; private set;} // declines slowly across convo if it reaches zero robot leaves (how long you're talking) /10
    public int Connection {get; private set;} // changes with the how the conversation is going (what youre talking about) /10
    public int Attraction {get; private set;} // once they meet first time (initial vibes) /10


    [Header("Tweaking/Balance")]
    [SerializeField] private int _attractionBalanceVariability;
    [SerializeField] private int _maxInitialInterestReduction;
    [SerializeField] private int _initialInterestAttractionVariability;
    [SerializeField] private float _conversationLengthBalanceMultiplier;
    [SerializeField] private float _isVibingInterestMultiplier;
    [SerializeField] private float _hasPatienceInterestMultiplier;
    [SerializeField] private float _lostPatienceInterestMultiplier;
    [SerializeField] private int _maxInterest = 10;
    [SerializeField] private int _maxConnection = 10;
    [SerializeField] private int _maxAttraction = 10;


    [Header("References")]
    [SerializeField] private Transform _textBoxPos;
    [SerializeField] private GameObject _dialogueBubblePrefab;


    [Header("Testing")]
    [SerializeField] private TextMeshProUGUI _testTextOutput;



    private int linesReceivedCount;
    private int linesSentCount;

    private bool isVibing;
    private bool lostPatience;
    private GameObject currentActiveTextBubble;



    void Start()
    {
        DialogueManager.Instance.AddActiveRobot(this);
    }

    public void ReceiveMatchedRobotCompatibility(int compatibilityStrength)
    {
        Attraction = compatibilityStrength + VariabilityFunction(0,0, _attractionBalanceVariability);
        if (Attraction > _maxAttraction) Attraction = _maxAttraction;
        if (Attraction < 0) Attraction = 0;

        InitialInterest();
        ShowValues(); 
    }

    private void InitialInterest()
    {
        int variedAttraction = Attraction + VariabilityFunction(0,0, _initialInterestAttractionVariability);
        Debug.Log($"variedAttraction: {variedAttraction}");
        int initialInterestReduction = Mathf.RoundToInt((10-variedAttraction) / 10f * _maxInitialInterestReduction);
        Debug.Log($"initialInterestReduction: {initialInterestReduction}");

        Interest = 10 - initialInterestReduction;

        if (Interest > _maxInterest) Interest = _maxInterest;
        if (Interest < 0) Interest = 0;
    }

    public void ReceiveConvoConnectionValueChange(int connectionChange)
    {
        Connection += connectionChange;
        if (Connection > _maxConnection) Connection = _maxConnection;
        if (Connection < 0) Connection = 0;

        linesReceivedCount++;
        CalculateInterest();

        ShowValues();
    }

    private void CalculateInterest()
    {
        int conversationLength = linesReceivedCount + linesSentCount;

        isVibing = Connection > CharacterData.Pickiness;
        lostPatience = ((conversationLength * _conversationLengthBalanceMultiplier) > CharacterData.Patience) && (isVibing == false);

        if (isVibing)
        {
            Interest -= Mathf.FloorToInt(conversationLength * _isVibingInterestMultiplier);
        }
        else if (lostPatience)
        {
            Interest -= Mathf.FloorToInt(conversationLength * _lostPatienceInterestMultiplier);
        }
        else
        {
            Interest -= Mathf.FloorToInt(conversationLength * _hasPatienceInterestMultiplier);
        }
        

        if (Interest > _maxInterest) Interest = _maxInterest;
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

    private int VariabilityFunction(int baseValue, int addedValue, int variability)
    {
        return baseValue + addedValue + Random.Range(-variability, variability);
    }

    private void DeleteActiveTextBubble()
    {
        currentActiveTextBubble.GetComponent<DialogueBox>().DeleteBubble();
        currentActiveTextBubble = null;
    }

    private void ShowValues()
    {
        _testTextOutput.text = $"Interest: {Interest}\nConnection: {Connection}\nAttraction: {Attraction}\nLines Sent: {linesSentCount}\nLines Received: {linesReceivedCount}\nIsVibing: {isVibing}\nLostPatience: {lostPatience}";
    }
}
