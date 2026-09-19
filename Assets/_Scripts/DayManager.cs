using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : Singleton<DayManager>
{
    public MatchBalanceSettings BalanceSettings;
    public int Coins {get; private set;}
    public int MatchedQuota {get; private set;}
    public int CurrentDayNum {get; private set;}
    public bool DayStarted {get; private set;}


    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        CurrentDayNum = 0;
        DialogueManager.Instance.ShowStoryPrompt();
        DialogueManager.Instance.ShowTutorialPrompt();
        DialogueManager.Instance.ShowStartGamePrompt();
        //GameUIManager.Instance.ToggleNewDayButton(true);
        //GameUIManager.Instance.ToggleContinueButton(false);
    }

    public void EndDay(int robotsMatched, bool didWin)
    {
        if (robotsMatched >= BalanceSettings.InitialMatchedCountQuota[CurrentDayNum-1])
        {
            UpdateCoins(BalanceSettings.QuotaCoinBonus);
        }
        
        if (CurrentDayNum >= BalanceSettings.TotalDaysCount)
        {
            DialogueManager.Instance.ShowGameOverPrompt(didWin);
            return;
        } 

        //GameUIManager.Instance.ToggleNewDayButton(true);
        //GameUIManager.Instance.ToggleContinueButton(false);

        DayStarted = false;
    }

    public void StartNewDay() //Called by button
    {
        MatchedQuota = BalanceSettings.InitialMatchedCountQuota[CurrentDayNum];
        Coins = BalanceSettings.InititalCoinAmount;

        DialogueManager.Instance.StartDaySetup(BalanceSettings.InitialRobotCountPerDay[CurrentDayNum]);
        CurrentDayNum++;

        //GameUIManager.Instance.ToggleNewDayButton(false);
        //GameUIManager.Instance.ToggleContinueButton(true);

        GameUIManager.Instance.DisplayDaysLeftText(BalanceSettings.TotalDaysCount - CurrentDayNum + 1); // includes current day
        GameUIManager.Instance.DisplayMatchedAndQuotaCountText(0, MatchedQuota);
        GameUIManager.Instance.DisplayCoinsCountText(Coins);
        GameUIManager.Instance.DisplayRejectCostText(BalanceSettings.RobotRejectCoinCost);
        GameUIManager.Instance.DisplayMatchPositiveGainText(BalanceSettings.PositiveMatchCoinGain);
        GameUIManager.Instance.DisplayMatchNegativeCostText(BalanceSettings.NegativeMatchCoinCost);

        DayStarted = true;
    }

    public void EndGame()
    {
        //GameUIManager.Instance.DisplayGameOver();
        SceneManager.LoadScene(0);
    }

    public void UpdateCoins(int coinChange)
    {
        Coins += coinChange;
        GameUIManager.Instance.DisplayCoinsCountText(Coins);

        //if (Coins <= 0) DialogueManager.Instance.RobotsFinished();
    }
}
