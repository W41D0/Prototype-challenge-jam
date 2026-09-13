using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 1)]
public class CharacterData : ScriptableObject
{
    public Names Name;
    public string Description;

    [Header("Personality")]
    public Personalites Personality;


    [Range(0,10)]
    public int Confidence; //increases odds for going first and talking more

    [Range(0,10)]
    public int Patience; //threshold for how long conversation can go before start losing interest

    [Range(0,10)]
    public int Pickiness; //if Connection > Pickiness then they Vibing
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
