using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textField;

    public void DisplayText(int textCount)
    {
        //take number and turn to robot gibberish
        _textField.text = textCount.ToString();
    }

    public void DeleteBubble()
    {
        Destroy(gameObject);
    }
}
