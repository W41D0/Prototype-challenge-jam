using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textField;

    public void DisplayText(string outputText)
    {
        _textField.text = outputText;
    }

    public void DeleteBubble()
    {
        Destroy(gameObject);
    }
}
