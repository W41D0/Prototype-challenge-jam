using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoiceManager : Singleton<VoiceManager>
{
    [SerializeField] private TextMeshProUGUI _voiceInYourHeadText;

    [SerializeField] private List<string> _generalNegativeLines; 
    [SerializeField] private List<string> _quitOneSidedNegativeLines; // when a robot quits and there was a robot in love
    [SerializeField] private List<string> _quitTwoSidedNegativeLines; // when a robot quits and they're both in Love
    [SerializeField] private List<string> _quitPositiveLines; // when a robot quits and they're both angry
    [SerializeField] private List<string> _rejectTwoSidedNegativeLines; // when you reject when they both Love each other
    [SerializeField] private List<string> _rejectOneSidedNegativeLines; // when you reject when only one of them is in love 
    [SerializeField] private List<string> _rejectPositiveLines; // when you reject when they both Hate each other
    [SerializeField] private List<string> _matchOneSidedNegativeLines; // when you match and one of them is in love and the other angry
    [SerializeField] private List<string> _matchOneSidedPositiveLines; // when you match and one of them is in love and the other neutral
    [SerializeField] private List<string> _matchTwoSidedNegativeLines; // when you match and they both Hate eachother
    [SerializeField] private List<string> _matchPositiveLines; // when you match and they're both in love
    // [SerializeField] private List<string> _questionDecisionNegativeLines; 
    // [SerializeField] private List<string> _gamblingDecisionNegativeLines;
    // [SerializeField] private List<string> _twoSidedPositiveLines; 
    // [SerializeField] private List<string> _questionDecisionPositiveLines;
    // [SerializeField] private List<string> _gamblingDecisionPositiveLines;
    
    public void DisplayVoiceText(List<string> lines)
    {
        _voiceInYourHeadText.text = lines[Random.Range(0, lines.Count)];
    }

    public void ThinkQuitOneSidedNegative()
    {
        DisplayVoiceText(_quitOneSidedNegativeLines);
    }
    public void ThinkQuitTwoSidedNegative()
    {
        DisplayVoiceText(_quitTwoSidedNegativeLines);
    }
    public void ThinkQuitPositive()
    {
        DisplayVoiceText(_quitPositiveLines);
    }
    public void ThinkRejectTwoSidedNegative()
    {
        DisplayVoiceText(_rejectTwoSidedNegativeLines);
    }
    public void ThinkRejectOneSidedNegative()
    {
        DisplayVoiceText(_rejectOneSidedNegativeLines);
    }
    public void ThinkRejectPositive()
    {
        DisplayVoiceText(_rejectPositiveLines);
    }
    public void ThinkGeneralNegative()
    {
        DisplayVoiceText(_generalNegativeLines);
    }
    public void ThinkMatchOneSidedNegative()
    {
        DisplayVoiceText(_matchOneSidedNegativeLines);
    }
    public void ThinkMatchTwoSidedNegative()
    {
        DisplayVoiceText(_matchTwoSidedNegativeLines);
    }
    public void ThinkMatchOneSidedPositive()
    {
        DisplayVoiceText(_matchOneSidedPositiveLines);
    }
    public void ThinkMatchPositive()
    {
        DisplayVoiceText(_matchPositiveLines);
    }
    // public void ThinkQuestionDecisionNegative()
    // {
    //     DisplayVoiceText(_questionDecisionNegativeLines);
    // }
    // public void ThinkGamblingDecisionNegative()
    // {
    //     DisplayVoiceText(_gamblingDecisionNegativeLines);
    // }
    // public void ThinkGamblingDecisionPositive()
    // {
    //     DisplayVoiceText(_gamblingDecisionPositiveLines);
    // }
    // public void ThinkQuestionDecisionPositive()
    // {
    //     DisplayVoiceText(_gamblingDecisionPositiveLines);
    // }

}
