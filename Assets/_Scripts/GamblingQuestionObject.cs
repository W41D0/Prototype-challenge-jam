using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GamblingQuestionObject : MonoBehaviour
{
    [SerializeField] private Button _agreeButton;
    [SerializeField] private Button _disagreeButton;
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private TextMeshProUGUI _rewardGainText;
    [SerializeField] private TextMeshProUGUI _fineCostText;
    private GamblingQuestion gamblingQuestion;

    public void InitializeQuestion(GamblingQuestion question)
    {
        gamblingQuestion = question;

        _questionText.text = question.QuestionText;
        _rewardGainText.text = question.Reward.ToString();
        _fineCostText.text = question.Fine.ToString();

        _agreeButton.onClick.AddListener(() => QuestionButton(true));
        _disagreeButton.onClick.AddListener(() => QuestionButton(false));

        if (question.HasTwoAnswers == false) _disagreeButton.gameObject.SetActive(false);
    }

    public void QuestionButton(bool input)
    {
        GamblingManager.Instance.QuestionAnswered(input, gamblingQuestion);
        Destroy(gameObject);
    }
}
