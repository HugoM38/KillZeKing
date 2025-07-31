using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public CharacterData characterData;
    public Button button;
    public Image iconImage;
    private CharacterSelectUI ui;

    public void Init(CharacterData data, CharacterSelectUI selectUI)
    {
        characterData = data;
        ui = selectUI;
         iconImage.sprite = data.characterIcon;
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        ui.ShowCharacter(characterData);
    }
}
