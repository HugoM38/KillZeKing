using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    public Image icon;
    public Text nameText;
    public Text statsText;
    public Text specialText;

    public void ShowCharacter(CharacterData data)
    {
        icon.sprite = data.characterIcon;
        nameText.text = data.characterName;
        statsText.text = $"Health: {data.maxHealth}\n" +
                         $"Damage: {data.damage}\n" +
                         $"Attack Range: {data.attackRange}\n" +
                         $"Movement Range: {data.movementRange}\n" +
                         $"Energy: {data.energy}";
        specialText.text = $"{data.specialName}\n{data.specialDescription}";
    }
}
