using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private RectTransform _dialogueTextTransform;
    [SerializeField] private float _dialogueBoxXOffset;

    private MatchBalanceSettings BalanceSettings => DayManager.Instance.BalanceSettings;
    private int OrderedEmojisCount => DialogueManager.Instance.OrderedEmojisCount;
    private string RandomCharSymbols => DialogueManager.Instance.RandomCharSymbols;

    private bool isTyping;
    private float typingTimer;
    private int characterCountTyped;
    private int charactersInLine;
    private string outputText = "";
    private int baseConnection;
    private int currentAudioTypingCount;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (isTyping == true && typingTimer > 0 && characterCountTyped < charactersInLine)
        {
            typingTimer -= Time.deltaTime;
        }
        else if (isTyping == true && typingTimer <= 0 && characterCountTyped < charactersInLine)
        {
            outputText += ConnectionBasedEmoji(baseConnection, OrderedEmojisCount);
            _textField.text = outputText.ToString();

            characterCountTyped++;
            typingTimer = Random.Range(BalanceSettings.TimeBetweenCharactersRange.x, BalanceSettings.TimeBetweenCharactersRange.y);

            if (currentAudioTypingCount >= BalanceSettings.AudioOnTypeCounter)
            {
                if (BalanceSettings.StopTypingAudio) audioSource.Stop();
                audioSource.pitch = Random.Range(BalanceSettings.PitchTypingSoundRange.x, BalanceSettings.PitchTypingSoundRange.y);
                audioSource.volume = Random.Range(BalanceSettings.VolumeTypingSoundRange.x, BalanceSettings.VolumeTypingSoundRange.y);

                audioSource.PlayOneShot(BalanceSettings.TypingSounds[Random.Range(0, BalanceSettings.TypingSounds.Length)]);
            }
            else
            {
                currentAudioTypingCount++;
            }
        }
        else if (isTyping == true && characterCountTyped >= charactersInLine)
        {
            isTyping = false;
            _textField.text = outputText.ToString();
        }
    }

    public void DisplayText(int textCount, int connection, int confidence, bool isRightSided)
    {
        //take number and turn to robot gibberish
        isTyping = true;
        typingTimer = Random.Range(BalanceSettings.TimeBetweenCharactersRange.x, BalanceSettings.TimeBetweenCharactersRange.y);
        baseConnection = connection;

        if (textCount > BalanceSettings.MaxTextCount) textCount = BalanceSettings.MaxTextCount;
        charactersInLine = textCount;

        


        // for (int i = 0; i < textCount; i++)
        // {
        //     if ((BalanceSettings.EmojiOnly) || (Random.value * 10) <= (confidence * BalanceSettings.EmojiConfidenceThresholdMultiplier))
        //     {
        //         outputText += ConnectionBasedEmoji(connection, OrderedEmojisCount);
        //     }
        //     else
        //     {
        //         int charIndex = Random.Range(0, RandomCharSymbols.Length);
        //         outputText += RandomCharSymbols[charIndex].ToString();
        //     }
        // }

        

        if (isRightSided == true) _dialogueTextTransform.anchoredPosition = new Vector2(_dialogueBoxXOffset, _dialogueTextTransform.anchoredPosition.y);
        else _dialogueTextTransform.anchoredPosition = new Vector2(-_dialogueBoxXOffset, _dialogueTextTransform.anchoredPosition.y);
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
