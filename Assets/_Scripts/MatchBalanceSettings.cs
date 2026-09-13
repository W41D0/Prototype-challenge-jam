using UnityEngine;

[CreateAssetMenu(fileName = "MatchBalanceSettings", menuName = "ScriptableObjects/MatchBalanceSettings", order = 1)]
public class MatchBalanceSettings : ScriptableObject
{
    [Header("Pair Settings")]
    public int TextCountBase = 10; //Base amount of characters in gibberish before adding Confidence
    public int TextCountVariability = 5; //Changes TextCountBase by random num between [-value,value]
    public int ConnectionRandomRollBaseChange = 3; //Random roll from [-value,value] that offsets robot Connection every convo update (value before Modifer and Variability)
    public int ConnectionBaseModifier = 0; //Unfairly biases all connection (could be positive or negative)
    public int ConnectionVariability = 1; //Changes ConnectionBaseModifier by random num between -value,value
    public int AttractionVariability = 2; //Changes Attraction by random num between -value,value

    [Header("Robot Settings")]
    public int AttractionBalanceVariability = 2; //Changes Attraction by random num between [-value,value]
    public int MaxInitialInterestReduction = 4; //Maximum possible Interest reduction from the default of 10
    public float ConversationLengthBalanceMultiplier = 0.08f; //Multiplies into conversation length before comparing to Patience
    public float HasPatienceInterestDecrement = 0.25f; //Subtracts from Interest every convo update if hasPatience 
    public float LostPatienceInterestDecrement = 0.5f; //Subtracts from Interest every convo update if lostPatience 
    public float IsVibingInterestDecrement = 0.125f; //Subtracts from Interest every convo update if isVibing 
    public int MaxInterest = 10;
    public int MaxConnection = 10;
    public int MaxAttraction = 10;
}
