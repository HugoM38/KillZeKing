using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterIcon;

    [Header("Stats")]
    public int maxHealth;
    public int damage;
    public int attackRange;
    public int movementRange;
    public int energy;

    [Header("Attaque spéciale")]
    public string specialName;
    [TextArea] public string specialDescription;
}
