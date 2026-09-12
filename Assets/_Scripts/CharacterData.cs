using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 1)]
public class CharacterData : ScriptableObject
{
    public Names Name;
    public string Description;

    [Header("Personality")]
    public Personalites Personality;
    public Personalites[] Likes;
    public Personalites[] Dislikes;

    [Range(0,10)]
    public int Confidence;
}


public enum Personalites
{
    Arrogant,
    Romantic,
    Edgy,
    Sleepy,
    // Desperate,
    // Crazy,
    // Nerd,
    // Monk,
}

public enum Names
{
    Adriano,
    Romanoff,
    Edgar,
    Sleppino,
    // Desperate,
    // Crazy,
    // Nerd,
    // Monk,
}
