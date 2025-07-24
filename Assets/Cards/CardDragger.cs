using UnityEngine;

public class CardDragger : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    // Limites de la zone (à ajuster selon la taille de ton terrain)
    public float minX = -8f, maxX = 8f;
    public float minY = -4f, maxY = 0f; // Ex: zone en bas de l’écran

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPos() + offset;
            // Clamp dans la zone autorisée
            float clampedX = Mathf.Clamp(mousePos.x, minX, maxX);
            float clampedY = Mathf.Clamp(mousePos.y, minY, maxY);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
