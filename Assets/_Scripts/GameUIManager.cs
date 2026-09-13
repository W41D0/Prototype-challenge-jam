using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameUIManager : Singleton<GameUIManager>
{
    [SerializeField] private RectTransform RosterButton;

    [SerializeField] private TextMeshProUGUI _testTextOutput0;
    [SerializeField] private TextMeshProUGUI _testTextOutput1;
    [SerializeField] private TextMeshProUGUI _robotsRejectedCountText;
    [SerializeField] private TextMeshProUGUI _robotsMatchedCountText;

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

    public void DisplayRejectedCount(int rejectedCount)
    {
        _robotsRejectedCountText.text = rejectedCount.ToString();
    }

    public void DisplayMatchedCount(int matchedCount)
    {
        _robotsMatchedCountText.text = matchedCount.ToString();
    }
}
