// Assets/Scripts/CardDefinition.cs
using UnityEngine;

public enum CardType { Spell, Monster, Trap }
public enum EffectType { Fireball, Heal, Buff /*…*/ }

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Definition")]
public class CardDefinition : ScriptableObject {
    public string      cardName;
    [TextArea] 
    public string      description;
    public Sprite      artwork;
    public CardType    cardType;
    public EffectType  effectType;
    public int         effectStrength = 1; // ex. dégâts ou soin
}
