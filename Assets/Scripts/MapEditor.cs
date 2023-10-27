using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MapEditor : MonoBehaviour
{
    public TMP_Dropdown tileDropdown;
    public List<GameObject> tilePrefabs;
    private GameObject selectedTilePrefab;

    private void Start()
    {
        PopulateDropdown();
        selectedTilePrefab = tilePrefabs[0];
        tileDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void PopulateDropdown()
    {
        List<string> options = new List<string>
        {
            "Water",
            "Steel",
            "Dirt",
            "Quicksand",
            "Brick"
        };

        tileDropdown.ClearOptions();
        tileDropdown.AddOptions(options);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectClickedTile();
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        string selectedTileName = tileDropdown.options[index].text;
        selectedTilePrefab = tilePrefabs.Find(tile => tile.name == selectedTileName);

        if (selectedTilePrefab == null)
        {
            Debug.LogError($"No matching tile prefab found for selected dropdown option: {selectedTileName}");
        }
    }

    private void DetectClickedTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

        if (hit.collider != null)
        {
            GameObject clickedTile = hit.collider.gameObject;
            Transform parentTransform = clickedTile.transform.parent;

            if (parentTransform != null)
            {
                string parentName = parentTransform.gameObject.name;

                if (parentName == "BarrierContainer")
                {
                    Debug.Log($"Raycast hit object in BarrierContainer");
                    ReplaceTile(clickedTile, parentTransform.gameObject);
                }
                else if (parentName == "GroundContainer")
                {
                    Debug.Log($"Raycast hit object in GroundContainer");
                    ReplaceTile(clickedTile, parentTransform.gameObject);
                }
                else
                {
                    Debug.Log($"Raycast hit object in unsupported container: {parentName}");
                }
            }
            else
            {
                Debug.Log("Raycast hit object with no parent.");
            }
        }
        else
        {
            Debug.Log("No tile was clicked.");
        }
    }

    private void ReplaceTile(GameObject oldTile, GameObject parentContainer)
    {
        Vector3 oldTilePosition = oldTile.transform.position;
        int oldOrderInLayer = oldTile.GetComponent<SpriteRenderer>().sortingOrder;
        Destroy(oldTile);
        GameObject newTile = Instantiate(selectedTilePrefab, oldTilePosition, Quaternion.identity);
        newTile.GetComponent<SpriteRenderer>().sortingOrder = oldOrderInLayer;
        newTile.transform.SetParent(parentContainer.transform);
    }
}
