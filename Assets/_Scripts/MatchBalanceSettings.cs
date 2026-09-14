using UnityEngine;

[CreateAssetMenu(fileName = "MatchBalanceSettings", menuName = "ScriptableObjects/MatchBalanceSettings", order = 1)]
public class MatchBalanceSettings : ScriptableObject
{
    [Header("Game Settings")]
    public int TotalDaysCount = 5;
    public int PositiveMatchCoinGain = 3;
    public int NegativeMatchCoinCost = -3;
    public int RobotQuitCoinCost = -3;
    public int RobotRejectCoinCost = -1;
    public int[] InitialMatchedCountQuota = new int[5] {2,3,4,4,5};
    public int QuotaCoinBonus = 10;
    public int[] InitialRobotCountPerDay = new int[5] {6,7,9,9,10};


    [Header("Pair Settings")]
    public int TextCountBase = 10; //Base amount of characters in gibberish before adding Confidence
    public int TextCountVariability = 5; //Changes TextCountBase by random num between [-value,value]
    public int ConnectionRandomRollBaseChange = 3; //Random roll from [-value,value] that offsets robot Connection every convo update (value before Modifer and Variability)
    public int ConnectionBaseModifier = 1; //Unfairly biases all connection (could be positive or negative)
    public int ConnectionVariability = 1; //Changes ConnectionBaseModifier by random num between -value,value
    public int AttractionVariability = 2; //Changes Attraction by random num between -value,value

    [Header("Robot Settings")]
    public int AttractionBalanceVariability = 2; //Changes Attraction by random num between [-value,value]
    public int MaxInitialInterestReduction = 4; //Maximum possible Interest reduction from the default of 10
    public float ConversationLengthBalanceMultiplier = 0.08f; //Multiplies into conversation length before comparing to Patience
    public float HasPatienceInterestDecrement = 0.25f; //Subtracts from Interest every convo update if hasPatience 
    public float IsAngryInterestDecrement = 0.5f; //Subtracts from Interest every convo update if IsAngry 
    public float IsInLoveInterestDecrement = 0.125f; //Subtracts from Interest every convo update if IsInLove 
    public int MaxInterest = 10;
    public int MaxConnection = 10;
    public int MaxAttraction = 10;
}
