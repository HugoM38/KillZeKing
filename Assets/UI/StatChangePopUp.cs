using UnityEngine;
using TMPro;

public class StatChangePopup : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float duration = 1f;
    public Vector3 moveDirection = Vector3.up;
    public float moveSpeed = 1f;

    public void Initialize(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
