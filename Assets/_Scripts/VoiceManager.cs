using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoiceManager : Singleton<VoiceManager>
{
    [SerializeField] private TextMeshProUGUI _voiceInYourHeadText;

    [SerializeField] private List<string> _generalNegativeLines; 
    [SerializeField] private List<string> _quitNegativeLines; 
    [SerializeField] private List<string> _rejectNegativeLines; 
    [SerializeField] private List<string> _rejectPositiveLines; 
    [SerializeField] private List<string> _oneSidedNegativeLines; 
    [SerializeField] private List<string> _twoSidedNegativeLines; 
    [SerializeField] private List<string> _questionDecisionNegativeLines; 
    [SerializeField] private List<string> _gamblingDecisionNegativeLines;
    [SerializeField] private List<string> _twoSidedPositiveLines; 
    [SerializeField] private List<string> _questionDecisionPositiveLines;
    [SerializeField] private List<string> _gamblingDecisionPositiveLines;
    
    public void DisplayVoiceText(List<string> lines)
    {
        _voiceInYourHeadText.text = lines[Random.Range(0, lines.Count)];
    }

    public void ThinkQuitNegative()
    {
        DisplayVoiceText(_quitNegativeLines);
    }
    public void ThinkRejectNegative()
    {
        DisplayVoiceText(_rejectNegativeLines);
    }
    public void ThinkRejectPositive()
    {
        DisplayVoiceText(_rejectPositiveLines);
    }
    public void ThinkGeneralNegative()
    {
        DisplayVoiceText(_generalNegativeLines);
    }
    public void ThinkOneSidedNegative()
    {
        DisplayVoiceText(_oneSidedNegativeLines);
    }
    public void ThinkTwoSidedNegative()
    {
        DisplayVoiceText(_twoSidedNegativeLines);
    }
    public void ThinkQuestionDecisionNegative()
    {
        DisplayVoiceText(_questionDecisionNegativeLines);
    }
    public void ThinkGamblingDecisionNegative()
    {
        DisplayVoiceText(_gamblingDecisionNegativeLines);
    }
    public void ThinkGamblingDecisionPositive()
    {
        DisplayVoiceText(_gamblingDecisionPositiveLines);
    }
    public void ThinkQuestionDecisionPositive()
    {
        DisplayVoiceText(_gamblingDecisionPositiveLines);
    }
    public void ThinkTwoSidedPositive()
    {
        DisplayVoiceText(_twoSidedPositiveLines);
    }
}
