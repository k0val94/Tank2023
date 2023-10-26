using UnityEngine;

public class MapEditor : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectClickedTile();
        }
    }

    private void DetectClickedTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (hit.collider != null)
        {
            GameObject clickedTile = hit.collider.gameObject;
            Debug.Log($"Clicked on tile: {clickedTile.name}");
        }
        else
        {
            Debug.Log("No tile was clicked.");
        }
    }
}