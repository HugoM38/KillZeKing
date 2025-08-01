using UnityEngine;

public class SummonManager : MonoBehaviour
{
    public static SummonManager Instance { get; private set; }

    [HideInInspector] public GameObject pendingPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Appelé par la carte Summon au clic.
    /// </summary>
    public void ActivateSummon(GameObject prefab)
    {
        pendingPrefab = prefab;
        Debug.Log($"Summon activated: {prefab.name}");
    }

    public void ClearSummon()
    {
        pendingPrefab = null;
    }
}
