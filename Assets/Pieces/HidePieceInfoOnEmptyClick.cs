using UnityEngine;

public class HidePieceInfoOnEmptyClick : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            // Raycast sur le monde 2D
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider == null || hit.collider.GetComponent<ChessPiece>() == null)
            {
                PieceInfoUI.Hide();
            }
        }
    }
}
