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

    [Header("Text Lines")]
    public int OrderedEmojisCount;
    public string RandomCharSymbols;

    [Header("Story/Tutorial lines")]
    [SerializeField] private List<string> _tutorialLines; 
    [SerializeField] private List<string> _storyLines; 

    [Header("References")]
    [SerializeField] private InputActionReference _continueInput;
    [SerializeField] private Robot _seat0Robot;
    [SerializeField] private Robot _seat1Robot;
    [SerializeField] private AudioClip _clickAudioClip;
    [SerializeField] private float _clickAudioClipVolume;
    [SerializeField] private float _clickAudioClipPitch;
    [SerializeField] private AudioClip _rejectAudioClip;
    [SerializeField] private float _rejectAudioClipVolume;
    [SerializeField] private float _rejectAudioClipPitch;
    [SerializeField] private AudioClip _matchAudioClip;
    [SerializeField] private float _matchAudioClipVolume;
    [SerializeField] private float _matchAudioClipPitch;
    [SerializeField] private AudioClip _mutateAudioClip;
    [SerializeField] private float _mutateAudioClipVolume;
    [SerializeField] private float _mutateAudioClipPitch;
    [SerializeField] private RectTransform _terminalTransform;
    [SerializeField] private GameObject _terminalContinueTextPre;
    [SerializeField] private GameObject _terminalGameOverLoseTextPre;
    [SerializeField] private GameObject _terminalGameOverWinTextPre;
    [SerializeField] private GameObject _terminalStartGameTextPre;
    [SerializeField] private GameObject _terminalTutorialTextPre;
    [SerializeField] private GameObject _terminalStoryTextPre;
    [SerializeField] private GameObject _terminalEmptyTextPre;
    
    [SerializeField] private List<CharacterData> _spawnableRobots;
    [SerializeField] private Sprite[] _redBatterySprites;
    [SerializeField] private Sprite[] _greenBatterySprites;
    [SerializeField] private Sprite[] _characterSprites;
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
    private int goodMatchesCount;
    private int robotsLeft;

    private bool isPaused;

    private float dialogueTimer;

    private bool inTutorial;
    private int currentTutorialLine;
    private float tutorialTypingTimer;
    private int tutorialCharactersInLine;
    private TextMeshProUGUI tutorialLineText;


    private bool inStory;
    private int currentStoryLine;
    private float storyTypingTimer;
    private int storyCharactersInLine;
    private TextMeshProUGUI storyLineText;

    private AudioSource audioSource;


    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {

        if (inTutorial == true && tutorialTypingTimer > 0 && tutorialLineText.maxVisibleCharacters < tutorialCharactersInLine)
        {
            tutorialTypingTimer -= Time.deltaTime;
        }
        else if (inTutorial == true && tutorialTypingTimer <= 0 && tutorialLineText.maxVisibleCharacters < tutorialCharactersInLine)
        {
            tutorialLineText.maxVisibleCharacters++;
            tutorialTypingTimer = BalanceSettings.TimeBetweenTutorialCharacters;
        }
        else if (inTutorial == true && tutorialLineText.maxVisibleCharacters >= tutorialCharactersInLine)
        {
            if (currentTutorialLine >= _tutorialLines.Count)
            {
                inTutorial = false;
            }
            // show other prompts
        }

        //Story
        if (inStory == true && storyTypingTimer > 0 && storyLineText.maxVisibleCharacters < storyCharactersInLine)
        {
            storyTypingTimer -= Time.deltaTime;
        }
        else if (inStory == true && storyTypingTimer <= 0 && storyLineText.maxVisibleCharacters < storyCharactersInLine)
        {
            storyLineText.maxVisibleCharacters++;
            storyTypingTimer = BalanceSettings.TimeBetweenStoryCharacters;
        }
        else if (inStory == true && storyLineText.maxVisibleCharacters >= storyCharactersInLine)
        {
            if (currentStoryLine >= _storyLines.Count)
            {
                inStory = false;
            }
            // show other prompts
        }




        if (!isPaused && dialogueTimer > 0)
        {
            dialogueTimer -= Time.deltaTime;
        }
        else if (dialogueTimer <= 0)
        {
            PressedContinue();
            dialogueTimer = Random.Range(BalanceSettings.TimeBetweenLinesRange.x, BalanceSettings.TimeBetweenLinesRange.y);
        }
    }



    public void PressedContinue()
    {
        if (isMatching && DayManager.Instance.DayStarted) CallNextDialogue();
        else if (!isMatching && DayManager.Instance.DayStarted) StartNewDate();
    }
    public void ShowContinuePrompt()
    {
        if (!isMatching) return;
        GameObject prompt = Instantiate(_terminalContinueTextPre, _terminalTransform);
        prompt.GetComponentInChildren<Button>().onClick.AddListener(PressedContinue);
    }

    public void ShowGameOverPrompt(bool didWin)
    {
        GameObject prompt = null;
        if (didWin) prompt = Instantiate(_terminalGameOverWinTextPre, _terminalTransform);
        else prompt = Instantiate(_terminalGameOverLoseTextPre, _terminalTransform);
        prompt.GetComponentInChildren<Button>().onClick.AddListener(DayManager.Instance.EndGame);
    }
    public void ShowStartGamePrompt()
    {
        GameObject prompt = Instantiate(_terminalStartGameTextPre, _terminalTransform);
        prompt.GetComponentInChildren<Button>().onClick.AddListener(DayManager.Instance.StartNewDay);
    }

    public void ShowTutorialPrompt()
    {
        GameObject prompt2 = Instantiate(_terminalTutorialTextPre, _terminalTransform);
        prompt2.GetComponentInChildren<Button>().onClick.AddListener(ShowNextTutorialLine);
    }

    public void ShowStoryPrompt()
    {
        GameObject prompt = Instantiate(_terminalStoryTextPre, _terminalTransform);
        prompt.GetComponentInChildren<Button>().onClick.AddListener(ShowNextStoryLine);
    }

    public void ShowNextTutorialLine()
    {
        if (currentTutorialLine >= _tutorialLines.Count) 
        {
            ShowStoryPrompt();
            ShowStartGamePrompt();
            return;
        }

        GameObject terminalLine = Instantiate(_terminalEmptyTextPre, _terminalTransform);
        terminalLine.GetComponentInChildren<Button>().onClick.AddListener(ShowNextTutorialLine);

        inTutorial = true;
        tutorialLineText = terminalLine.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        tutorialLineText.text = _tutorialLines[currentTutorialLine];
        tutorialCharactersInLine = _tutorialLines[currentTutorialLine].Length;
        tutorialLineText.maxVisibleCharacters = 0;

        currentTutorialLine++;

        audioSource.pitch = _clickAudioClipPitch;
        audioSource.volume = _clickAudioClipVolume;
        audioSource.PlayOneShot(_clickAudioClip);
    }

    public void ShowNextStoryLine()
    {
        if (currentStoryLine >= _storyLines.Count) 
        {
            ShowTutorialPrompt();
            ShowStartGamePrompt();
            return;
        }

        GameObject terminalLine = Instantiate(_terminalEmptyTextPre, _terminalTransform);
        terminalLine.GetComponentInChildren<Button>().onClick.AddListener(ShowNextStoryLine);

        inStory = true;
        storyLineText = terminalLine.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        storyLineText.text = _storyLines[currentStoryLine];
        storyCharactersInLine = _storyLines[currentStoryLine].Length;
        storyLineText.maxVisibleCharacters = 0;

        currentStoryLine++;

        audioSource.pitch = _clickAudioClipPitch;
        audioSource.volume = _clickAudioClipVolume;
        audioSource.PlayOneShot(_clickAudioClip);
    }



    public void StartDaySetup(int initialRobotCount)
    {
        robotsLeft = initialRobotCount;
        GameUIManager.Instance.DisplayRobotsLeftText(robotsLeft);
        isPaused = true;
        dialogueTimer = 0;

        ActiveRobotsDict.Add(0, _seat0Robot);
        ActiveRobotsDict.Add(1, _seat1Robot);

        StartNewDate();
    }

    public void StartNewDate()
    {        
        GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[1]);

        robotsLeft -= 2;
        GameUIManager.Instance.DisplayGreenBatteryIconImage(_greenBatterySprites[robotsLeft/2]);

        InitializeRobots();
        isMatching = true;
        isPaused = false;

        ActiveRobotsDict[0].MutateAnim();
        ActiveRobotsDict[1].MutateAnim();
    }

    public void RobotsFinished()
    {
        //DayManager.Instance.EndDay(matchedCount);
        
        rejectedCount = 0;
        quitCount = 0;
        matchedCount = 0;
        goodMatchesCount = 0;
        robotsLeft = 0;

        GameUIManager.Instance.DisplayRejectedCountText(rejectedCount);
        GameUIManager.Instance.DisplayQuitCountText(quitCount);
        GameUIManager.Instance.DisplayRobotsLeftText(robotsLeft);
        GameUIManager.Instance.DisplayRobotValuesTextForSeat(0, "");
        GameUIManager.Instance.DisplayRobotValuesTextForSeat(1, "");
    }

    public bool IsGoodMatch()
    {
        if (ActiveRobotsDict[0].IsInLove && ActiveRobotsDict[1].IsInLove) return true;
        return false;
    }

    public bool SomeoneIsInLove()
    {
        if (ActiveRobotsDict[0].IsInLove || ActiveRobotsDict[1].IsInLove) return true;
        return false;
    }

    public bool IsOneSided()
    {
        if (SomeoneIsInLove() && !IsGoodMatch()) return true;
        return false;
    }

    public bool IsOneSidedNoAnger()
    {
        if (IsOneSided() && !ActiveRobotsDict[0].IsAngry && !ActiveRobotsDict[1].IsAngry) return true;
        return false;
    }
    public bool RedBatteryIsFull()
    {
        if (goodMatchesCount >= 3) return true;
        return false;
    }
    public bool GreenBatteryIsEmpty()
    {
        if (robotsLeft <= 0) return true;
        return false;
    }

    public void MatchRobotPairing() //Called By Button
    {
        if (ActiveRobotsDict.TryGetValue(0, out Robot robotInSeat0) == false) return;
        if (ActiveRobotsDict.TryGetValue(1, out Robot robotInSeat1) == false) return;

        if (IsGoodMatch())
        {
            goodMatchesCount++;
            VoiceManager.Instance.ThinkMatchPositive();
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[2]);
            GameUIManager.Instance.DisplayRedBatteryIconImage(_redBatterySprites[goodMatchesCount]);

            audioSource.pitch = _matchAudioClipPitch;
            audioSource.volume = _matchAudioClipVolume;
            audioSource.PlayOneShot(_matchAudioClip);

            if (RedBatteryIsFull()) DialogueManager.Instance.ShowGameOverPrompt(true);
        } 
        else if (IsOneSidedNoAnger())
        {
            goodMatchesCount++;
            VoiceManager.Instance.ThinkMatchOneSidedPositive();
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[2]);
            GameUIManager.Instance.DisplayRedBatteryIconImage(_redBatterySprites[goodMatchesCount]);

            audioSource.pitch = _matchAudioClipPitch;
            audioSource.volume = _matchAudioClipVolume;
            audioSource.PlayOneShot(_matchAudioClip);

            if (RedBatteryIsFull()) DialogueManager.Instance.ShowGameOverPrompt(true);
        }
        else if (IsOneSided())
        {
            VoiceManager.Instance.ThinkMatchOneSidedNegative();
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[0]);

            audioSource.pitch = _rejectAudioClipPitch;
            audioSource.volume = _rejectAudioClipVolume;
            audioSource.PlayOneShot(_rejectAudioClip);

            robotsLeft -= 2;
        } 
        else
        {
            VoiceManager.Instance.ThinkMatchTwoSidedNegative();
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[0]);

            audioSource.pitch = _rejectAudioClipPitch;
            audioSource.volume = _rejectAudioClipVolume;
            audioSource.PlayOneShot(_rejectAudioClip);

            robotsLeft -= 2;
        } 


        if (GreenBatteryIsEmpty()) DialogueManager.Instance.ShowGameOverPrompt(false);
        else ShowContinuePrompt();

        isPaused = true;
        isMatching = false;

        ActiveRobotsDict[0].MutateAnim();
        ActiveRobotsDict[1].MutateAnim();

    
        DayManager.Instance.UpdateCoins(BalanceSettings.PositiveMatchCoinGain);

        matchedCount++;
        GameUIManager.Instance.DisplayMatchedAndQuotaCountText(matchedCount, DayManager.Instance.MatchedQuota);
    }

    public void MutateRobots()
    {
        if (ActiveRobotsDict.TryGetValue(0, out Robot robotInSeat0) == false) return;
        if (ActiveRobotsDict.TryGetValue(1, out Robot robotInSeat1) == false) return;


        if (IsGoodMatch())
        {
            VoiceManager.Instance.ThinkRejectTwoSidedNegative();
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[0]);

            ActiveRobotsDict[0].BreakHeart();
            ActiveRobotsDict[1].BreakHeart();

            audioSource.pitch = _rejectAudioClipPitch;
            audioSource.volume = _rejectAudioClipVolume;
            audioSource.PlayOneShot(_rejectAudioClip);
        } 
        else if (IsOneSided())
        {
            VoiceManager.Instance.ThinkRejectOneSidedNegative();
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[0]);

            if (ActiveRobotsDict[0].IsInLove) ActiveRobotsDict[0].BreakHeart();
            else ActiveRobotsDict[1].BreakHeart();  

            audioSource.pitch = _rejectAudioClipPitch;
            audioSource.volume = _rejectAudioClipVolume;
            audioSource.PlayOneShot(_rejectAudioClip);   
        } 
        else
        {
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[1]);
            VoiceManager.Instance.ThinkRejectPositive();

            audioSource.pitch = _mutateAudioClipPitch;
            audioSource.volume = _mutateAudioClipVolume;
            audioSource.PlayOneShot(_mutateAudioClip);

            robotsLeft += 2;
        } 

        if (GreenBatteryIsEmpty()) DialogueManager.Instance.ShowGameOverPrompt(false);
        else ShowContinuePrompt();

        isPaused = true;
        isMatching = false;

        ActiveRobotsDict[0].MutateAnim();
        ActiveRobotsDict[1].MutateAnim();

        rejectedCount++;
        GameUIManager.Instance.DisplayRejectedCountText(rejectedCount);

        DayManager.Instance.UpdateCoins(BalanceSettings.RobotRejectCoinCost);
    }

    public void RobotQuitInSeat(int seatNum)
    {
        if (IsGoodMatch()) 
        { 
            VoiceManager.Instance.ThinkQuitTwoSidedNegative();
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[0]);

            audioSource.pitch = _rejectAudioClipPitch;
            audioSource.volume = _rejectAudioClipVolume;
            audioSource.PlayOneShot(_rejectAudioClip);   
        }
        else if (IsOneSided())
        {
            VoiceManager.Instance.ThinkQuitOneSidedNegative();
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[0]);

            int otherbot = (seatNum == 0) ? 1 : 0;
            ActiveRobotsDict[otherbot].BreakHeart();

            audioSource.pitch = _rejectAudioClipPitch;
            audioSource.volume = _rejectAudioClipVolume;
            audioSource.PlayOneShot(_rejectAudioClip);   
        } 
        else
        {
            VoiceManager.Instance.ThinkQuitPositive();
            GameUIManager.Instance.DisplayCharacterIconImage(_characterSprites[1]);

            audioSource.pitch = _rejectAudioClipPitch;
            audioSource.volume = _rejectAudioClipVolume;
            audioSource.PlayOneShot(_rejectAudioClip);   
        } 

        if (GreenBatteryIsEmpty()) DialogueManager.Instance.ShowGameOverPrompt(false);
        else ShowContinuePrompt();

        isPaused = true;
        isMatching = false;


        quitCount++;
        GameUIManager.Instance.DisplayQuitCountText(quitCount);

        isPaused = true;

        DayManager.Instance.UpdateCoins(BalanceSettings.RobotQuitCoinCost);
    }

    private bool CanMakeNewPair()
    {
        if (robotsLeft >= 2) return true;
        if ((robotsLeft == 1) && (ActiveRobotsDict.Count != 0)) return true;

        return false;
    }

    private void InitializeRobots()
    {
        ActiveRobotsDict[0].InitializeRobot(_spawnableRobots[Random.Range(0, _spawnableRobots.Count - 1)], 0);
        ActiveRobotsDict[1].InitializeRobot(_spawnableRobots[Random.Range(0, _spawnableRobots.Count - 1)], 1);

        currentCompatibilityStrength = GetCompatibility(ActiveRobotsDict[0].CharacterData.Personality, ActiveRobotsDict[1].CharacterData.Personality);

        int variedCompatibility = currentCompatibilityStrength + VariabilityFunction(0, BalanceSettings.AttractionVariability);
        ActiveRobotsDict[0].ReceiveMatchedRobotCompatibility(variedCompatibility);
        

        variedCompatibility = currentCompatibilityStrength + VariabilityFunction(0, BalanceSettings.AttractionVariability);
        ActiveRobotsDict[1].ReceiveMatchedRobotCompatibility(variedCompatibility);
    }

    public void CallNextDialogue()
    {
        if (ActiveRobotsDict.TryGetValue(0, out Robot robotInSeat) == false) return;
        if (ActiveRobotsDict.TryGetValue(1, out robotInSeat) == false) return;

        int speakingBotIndex = RollConfidenceForOrder();
        int listeningBotIndex = (speakingBotIndex == 0) ? 1 : 0;

        int confidenceValue = ActiveRobotsDict[speakingBotIndex].CharacterData.Confidence;
        int textCount = VariabilityFunction(BalanceSettings.TextCountBase + confidenceValue, BalanceSettings.TextCountVariability);


        ActiveRobotsDict[speakingBotIndex].SayDialogue(textCount);


        int randomRoll = Mathf.RoundToInt(Random.Range(-BalanceSettings.ConnectionRandomRollBaseChange, BalanceSettings.ConnectionRandomRollBaseChange));
        randomRoll += VariabilityFunction(BalanceSettings.ConnectionBaseModifier, BalanceSettings.ConnectionVariability);
        ActiveRobotsDict[listeningBotIndex].ReceiveConvoConnectionValueChange(randomRoll);

        // if (ActiveRobotsDict.TryGetValue(0, out robotInSeat) == false) return;
        // if (ActiveRobotsDict.TryGetValue(1, out robotInSeat) == false) return;

        //randomRoll = Mathf.RoundToInt(Random.Range(-BalanceSettings.ConnectionRandomRollBaseChange, BalanceSettings.ConnectionRandomRollBaseChange));
        //randomRoll += VariabilityFunction(BalanceSettings.ConnectionBaseModifier, BalanceSettings.ConnectionVariability);
        //ActiveRobotsDict[speakingBotIndex].ReceiveConvoConnectionValueChange(randomRoll);
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
}


[System.Serializable]
public class PairingCompatibility
{
    public Personalites A;
    public Personalites B;
    [Range(0, 10)]
    public int Strength;
}