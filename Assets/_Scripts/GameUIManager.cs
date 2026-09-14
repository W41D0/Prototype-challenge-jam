using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameUIManager : Singleton<GameUIManager>
{
    //[SerializeField] private RectTransform RosterButton;
    
    [SerializeField] private TextMeshProUGUI _daysLeftText;
    [SerializeField] private TextMeshProUGUI _robotsMatchedAndQuotaText;
    [SerializeField] private TextMeshProUGUI _robotsRejectedText;
    [SerializeField] private TextMeshProUGUI _robotsQuitText;
    [SerializeField] private TextMeshProUGUI _robotsLeftText;
    [SerializeField] private TextMeshProUGUI _coinsCountText;

    [SerializeField] private TextMeshProUGUI _rejectCostSeat0Text;
    [SerializeField] private TextMeshProUGUI _rejectCostSeat1Text;
    [SerializeField] private TextMeshProUGUI _matchNegativeCostText;
    [SerializeField] private TextMeshProUGUI _matchPositiveGainText;

    [SerializeField] private GameObject _newDayButton;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _gameOverText;

    [SerializeField] private TextMeshProUGUI _testTextOutput0;
    [SerializeField] private TextMeshProUGUI _testTextOutput1;


    public void ToggleNewDayButton(bool onOff)
    {
        _newDayButton.SetActive(onOff);
    }

    public void ToggleContinueButton(bool onOff)
    {
        _continueButton.SetActive(onOff);
    }

    public void DisplayGameOver()
    {
        _gameOverText.SetActive(true);
    }

    public void DisplayMatchPositiveGainText(int gain)
    {
        _matchPositiveGainText.text = gain.ToString();
    }
    public void DisplayMatchNegativeCostText(int cost)
    {
        _matchNegativeCostText.text = cost.ToString();
    }
    public void DisplayRejectCostText(int rejectCost)
    {
        _rejectCostSeat0Text.text = rejectCost.ToString();
        _rejectCostSeat1Text.text = rejectCost.ToString();
    }
    public void DisplayDaysLeftText(int daysLeft)
    {
        _daysLeftText.text = daysLeft.ToString();
    }
    public void DisplayMatchedAndQuotaCountText(int matchedCount, int quota)
    {
        _robotsMatchedAndQuotaText.text = matchedCount.ToString() + "/" + quota.ToString();
    }
    public void DisplayRejectedCountText(int rejectedCount)
    {
        _robotsRejectedText.text = rejectedCount.ToString();
    }
    public void DisplayQuitCountText(int quitCount)
    {
        _robotsQuitText.text = quitCount.ToString();
    }
    public void DisplayRobotsLeftText(int robotsLeft)
    {
        _robotsLeftText.text = robotsLeft.ToString();
    }
    public void DisplayCoinsCountText(int coinsCount)
    {
        _coinsCountText.text = coinsCount.ToString();
    }

    public void DisplayRobotValuesTextForSeat(int seatNum, string valueText)
    {
        if (seatNum == 0)
        {
            _testTextOutput0.text = valueText;
        }
        else if (seatNum == 1)
        {
            _testTextOutput1.text = valueText;
        }
    }
}
