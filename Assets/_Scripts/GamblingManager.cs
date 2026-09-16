using System.Collections.Generic;
using UnityEngine;

public class GamblingManager : Singleton<GamblingManager>
{

    [SerializeField] private List<GamblingQuestion> _gamblingQuestions;
    [SerializeField] private GameObject _questionObjectPre;
    [SerializeField] private RectTransform _questionsObjectParent;
    [SerializeField] private int _questionsCount;


    private bool menuIsOn = false;

    public void SpawnQuestions()
    {
        if (menuIsOn)
        {
            menuIsOn = false;
            return;
        }

        menuIsOn = true;

        if (DialogueManager.Instance.ActiveRobotsDict.Count != 2) return;

        foreach (Transform question in _questionsObjectParent)
        {
            Destroy(question.gameObject);
        }
        

        List<GamblingQuestion> questionBin = new(_gamblingQuestions);

        for (int i = 0; i < _questionsCount; i++)
        {
            int randomIndex = Random.Range(0, questionBin.Count-1);

            GameObject questionObj = Instantiate(_questionObjectPre, _questionsObjectParent);
            questionObj.GetComponent<GamblingQuestionObject>().InitializeQuestion(questionBin[randomIndex]);
            questionBin.RemoveAt(randomIndex);
        }
    }


    public void QuestionAnswered(bool input, GamblingQuestion question)
    {
        if (AnswerGamblingQuestion(question) == input)
        {
            Debug.Log($"correct it was {input}");
            DayManager.Instance.UpdateCoins(question.Reward);

            if (question.IsCompleteLuck) VoiceManager.Instance.ThinkGamblingDecisionPositive();
            else VoiceManager.Instance.ThinkQuestionDecisionPositive();
        }
        else if (AnswerGamblingQuestion(question) != input) 
        {
            Debug.Log($"incorrect it was {!input}");
            DayManager.Instance.UpdateCoins(question.Fine);

            if (question.IsCompleteLuck) VoiceManager.Instance.ThinkGamblingDecisionNegative();
            else VoiceManager.Instance.ThinkQuestionDecisionNegative();
        }
    }

    public bool AnswerGamblingQuestion(GamblingQuestion question)
    {
        if (DialogueManager.Instance.ActiveRobotsDict.Count != 2) return false;

        switch (question.Question)
        {
            case QuestionName.OneOfThemIsInLove:
                return OneOfThemIsInLoveCheck();

            case QuestionName.OneOfThemIsAngry:
                return OneOfThemIsInAngryCheck();

            case QuestionName.RollADice:
                return RollADiceCheck();

            case QuestionName.PersonalityMatch:
                return PersonalityMatchCheck();
                
            default:
                return false;
        }
    }

    private bool OneOfThemIsInLoveCheck()
    {
        if (DialogueManager.Instance.ActiveRobotsDict[0].IsInLove == true) return true;
        if (DialogueManager.Instance.ActiveRobotsDict[1].IsInLove == true) return true;

        return false;
    }

    private bool OneOfThemIsInAngryCheck()
    {
        if (DialogueManager.Instance.ActiveRobotsDict[0].IsAngry == true) return true;
        if (DialogueManager.Instance.ActiveRobotsDict[1].IsAngry == true) return true;

        return false;
    }

    private bool RollADiceCheck()
    {
        if (Random.Range(1,7) == 6) return true;

        return false;
    }

    private bool PersonalityMatchCheck()
    {
        Personalites personality0 = DialogueManager.Instance.ActiveRobotsDict[0].CharacterData.Personality;
        Personalites personality1 = DialogueManager.Instance.ActiveRobotsDict[1].CharacterData.Personality;

        if (DialogueManager.Instance.GetCompatibility(personality0, personality1) >= 5) return true;

        return false;
    }

}

[System.Serializable]
public class GamblingQuestion
{
    public QuestionName Question;
    public string QuestionText;
    public int Reward;
    public int Fine;
    public bool HasTwoAnswers;
    public bool IsCompleteLuck;
}

public enum QuestionName
{
    OneOfThemIsInLove,
    OneOfThemIsAngry,
    RollADice,
    PersonalityMatch,

} 