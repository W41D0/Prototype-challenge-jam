using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Robot : MonoBehaviour
{

    [Header("Personality")]
    [field: SerializeField] public CharacterData CharacterData {get; private set;}
    public int Interest {get; private set;}
    public int Attraction {get; private set;}
    public int Connection {get; private set;}


    [Header("References")]
    [SerializeField] private Transform _textBoxPos;
    [SerializeField] private GameObject _dialogueBubblePrefab;



    private GameObject currentActiveTextBubble;

    void Start() //so it happens after DialogueManger's awake
    {
        DialogueManager.Instance.AddActiveRobot(this);
    }

    public void SayDialogue(string dialogue)
    {
        if (currentActiveTextBubble != null) DeleteActiveTextBubble();

        GameObject textBubble = Instantiate(_dialogueBubblePrefab, _textBoxPos.position, Quaternion.identity, _textBoxPos);
        textBubble.GetComponent<DialogueBox>().DisplayText(dialogue);

        currentActiveTextBubble = textBubble;
    }

    private void DeleteActiveTextBubble()
    {
        currentActiveTextBubble.GetComponent<DialogueBox>().DeleteBubble();
        currentActiveTextBubble = null;
    }
}
