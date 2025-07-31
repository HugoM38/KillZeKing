using UnityEngine;

public class CharacterMenuManager : MonoBehaviour
{
    public CharacterData[] characters;
    public GameObject buttonPrefab;
    public Transform buttonParent;
    public CharacterSelectUI infoUI;

    void Start()
    {
        foreach (var character in characters)
        {
            var btn = Instantiate(buttonPrefab, buttonParent);
            btn.GetComponent<CharacterButton>().Init(character, infoUI);
        }
    }
}
