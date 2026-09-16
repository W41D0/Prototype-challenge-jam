using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textField;

    private MatchBalanceSettings BalanceSettings => DayManager.Instance.BalanceSettings;
    private int OrderedEmojisCount => DialogueManager.Instance.OrderedEmojisCount;
    private string RandomCharSymbols => DialogueManager.Instance.RandomCharSymbols;

    public void DisplayText(int textCount, int connection, int confidence)
    {
        //take number and turn to robot gibberish
        string outputText = "";

        for (int i = 0; i < textCount; i++)
        {
            if ((BalanceSettings.EmojiOnly) || (Random.value * 10) <= (confidence * BalanceSettings.EmojiConfidenceThresholdMultiplier))
            {
                outputText += ConnectionBasedEmoji(connection, OrderedEmojisCount);
            }
            else
            {
                int charIndex = Random.Range(0, RandomCharSymbols.Length);
                outputText += RandomCharSymbols[charIndex].ToString();
            }
        }

        _textField.text = outputText.ToString();
    }

    private string ConnectionBasedEmoji(int connection, int emojiCount)
    {
        int emojiIndex = Mathf.RoundToInt(connection * (emojiCount-1) / 10) + VariabilityFunction(0, BalanceSettings.EmojiConnectionVariability);
        emojiIndex = Mathf.Clamp(emojiIndex, 0, emojiCount-1);
        return $"<sprite={emojiIndex}>";
    }

    public void DeleteBubble()
    {
        Destroy(gameObject);
    }

    float RemapRange(float value, (float, float) oldRange, (float, float) newRange)
    {
        return newRange.Item1 + (value - oldRange.Item1) / (oldRange.Item2 - oldRange.Item1) * (newRange.Item2 - newRange.Item1);
    }

    private int VariabilityFunction(int baseValue, int variability)
    {
        return baseValue + Random.Range(-variability, variability);
    }

}
