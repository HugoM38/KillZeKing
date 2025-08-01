using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public CharacterData characterData;
    public Button button;
    public Image iconImage;
    private CharacterSelectUI ui;
    public GameObject selectedHero;

    public void Init(CharacterData data, CharacterSelectUI selectUI)
    {
        characterData = data;
        ui = selectUI;
        iconImage.sprite = data.characterIcon;
        selectedHero = data.characterPrefab;
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        ui.ShowCharacter(characterData);
        CharacterSelectionMenu menu = FindObjectOfType<CharacterSelectionMenu>();
        menu.SelectHero(selectedHero);
    }
}
