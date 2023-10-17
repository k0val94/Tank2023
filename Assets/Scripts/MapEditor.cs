using UnityEngine;

public class MapEditor : MonoBehaviour
{
    public LayerMask groundMask; // Set this to the layer your ground is on

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && TileSelector.currentTile != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
            {
                Instantiate(TileSelector.currentTile, hit.point, Quaternion.identity);
            }
        }
    }
}
