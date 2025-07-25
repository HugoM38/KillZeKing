using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance { get; private set; }

    [HideInInspector] public string pendingSpellName = "";
    [HideInInspector] public int    pendingSpellValue = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ActivateSpell(string spellName, int value)
    {
        pendingSpellName  = spellName;
        pendingSpellValue = value;
        Debug.Log($"Sort activé : {spellName} ({value})");
    }

    public void ClearSpell()
    {
        pendingSpellName  = "";
        pendingSpellValue = 0;
    }
}
