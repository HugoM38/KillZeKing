// Assets/Scripts/EvolutionManager.cs
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager Instance { get; private set; }

    [HideInInspector] public GameObject pendingEvolutionPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Appelé par une carte Évolution au clic (ou au drop).
    /// </summary>
    public void ActivateEvolution(GameObject evoPrefab)
    {
        pendingEvolutionPrefab = evoPrefab;
        Debug.Log($"Evolution ready: {evoPrefab.name}");
    }

    public void ClearEvolution()
    {
        pendingEvolutionPrefab = null;
    }
}
