using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections;

public class Robot : MonoBehaviour
{
    [Header("Personality")]
    [field: SerializeField] public CharacterData CharacterData {get; private set;}
    public float Interest {get; private set;} // declines slowly across convo if it reaches zero robot leaves (how long you're talking) /10
    public int Connection {get; private set;} // changes with the how the conversation is going (what youre talking about) /10
    public int Attraction {get; private set;} // once they meet first time (initial vibes) /10

    public bool IsInLove {get; private set;}
    public bool IsAngry {get; private set;}

    [Header("References")]
    [SerializeField] private RectTransform _textBoxTransform;
    [SerializeField] private GameObject _dialogueBubblePrefab;
    [SerializeField] private List<Sprite> _orderedExpressionSprites;
    [SerializeField] private Animator _heartbeatAnim;
    [SerializeField] private Animator _robotAnim;





    private int linesReceivedCount;
    private int linesSentCount;
    private int seatNumber;
    private SpriteRenderer sr;

    private MatchBalanceSettings BalanceSettings => DayManager.Instance.BalanceSettings;




    public void InitializeRobot(CharacterData characterData, int seatNum)
    {
        ResetRobot();

        CharacterData = characterData;
        seatNumber = seatNum;


        sr = gameObject.GetComponent<SpriteRenderer>();
        if (seatNumber == 1) sr.flipX = true;
    }

    private void ResetRobot()
    {
        linesReceivedCount = 0;
        linesSentCount = 0;

        Interest = 0;
        Connection = 0;
        Attraction = 0;

        IsInLove = false;
        IsAngry = false;

        _heartbeatAnim.SetBool("Heartbroken", false);

        CharacterData = null;
    }

    public void MutateAnim()
    {
        StartCoroutine(MutateAnimTimer());
    }

    public void BreakHeart()
    {
        _heartbeatAnim.SetBool("Heartbroken", true);
    }

    private IEnumerator MutateAnimTimer()
    {
        _robotAnim.enabled = true;
        _robotAnim.SetTrigger("ScreenWipe");
        yield return new WaitForSeconds(0.6f);

        _robotAnim.enabled = false;
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

        if (Interest == 0)
        {
            DialogueManager.Instance.RobotQuitInSeat(seatNumber);
        } 

        Expression(Connection);

        ShowValues();
    }

    private void CalculateInterest()
    {
        int conversationLength = linesReceivedCount + linesSentCount;

        IsInLove = Connection > CharacterData.Pickiness;
        IsAngry = ((conversationLength * BalanceSettings.ConversationLengthBalanceMultiplier) > CharacterData.Patience) && (IsInLove == false);

        if (IsInLove)
        {
            Interest -= BalanceSettings.IsInLoveInterestDecrement;
        }
        else if (IsAngry)
        {
            Interest -= BalanceSettings.IsAngryInterestDecrement;
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
        bool isRightSided = true;
        if (seatNumber == 0) isRightSided = false;

        GameObject textBubble = Instantiate(_dialogueBubblePrefab, _textBoxTransform);
        textBubble.GetComponent<DialogueBox>().DisplayText(textCount, Connection, CharacterData.Confidence, isRightSided);

        linesSentCount++;
    }

    private void Expression(int connection)
    {
        int expressionIndex = Mathf.RoundToInt(connection * (_orderedExpressionSprites.Count-1) / 10) + VariabilityFunction(0, BalanceSettings.ExpressionConnectionVariability);
        expressionIndex = Mathf.Clamp(expressionIndex, 0, _orderedExpressionSprites.Count-1);
        
        sr.sprite = _orderedExpressionSprites[expressionIndex];
    }

    private int VariabilityFunction(int baseValue, int variability)
    {
        return baseValue + Random.Range(-variability, variability);
    }

    private void ShowValues()
    {
       string text = $"Interest: {Interest}\nConnection: {Connection}\nAttraction: {Attraction}\nLines Sent: {linesSentCount}\nLines Received: {linesReceivedCount}\nIsInLove: {IsInLove}\nIsAngry: {IsAngry}";
        GameUIManager.Instance.DisplayRobotValuesTextForSeat(seatNumber, text);
    }
}
