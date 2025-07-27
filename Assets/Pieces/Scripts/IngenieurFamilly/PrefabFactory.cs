using UnityEngine;

public class PrefabFactory : MonoBehaviour
{
    public static PrefabFactory Instance;

    [Header("Préfabriqués d'invocation")]
    public GameObject BarricadePrefab;
    public GameObject ShieldPrefab;
    public GameObject TurretPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
