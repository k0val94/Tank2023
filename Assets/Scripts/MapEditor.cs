using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MapEditor : MonoBehaviour
{
    public TMP_Dropdown tileDropdown;
    public List<GameObject> tilePrefabs;
    private GameObject selectedTilePrefab;
    
    private readonly List<string> tileOptions = new List<string>
    {
        "Water", "Steel", "Dirt", "Quicksand", "Brick"
    };

    private void Start()
    {
        InitializeDropdown();
        InitializeTileSelection();
    }

    private void Update()
    {
        DetectAndReplaceClickedTile();
    }

    private void InitializeDropdown()
    {
        tileDropdown.ClearOptions();
        tileDropdown.AddOptions(tileOptions);
        tileDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void InitializeTileSelection()
    {
        SelectTile(0);
    }

    private void OnDropdownValueChanged(int index)
    {
        SelectTile(index);
    }

    private void SelectTile(int index)
    {
        string selectedTileName = tileDropdown.options[index].text;
        selectedTilePrefab = tilePrefabs.Find(tile => tile.name == selectedTileName);

        if (selectedTilePrefab == null)
        {
            Debug.LogError($"No matching tile prefab found for selected dropdown option: {selectedTileName}");
        }
    }

    private void DetectAndReplaceClickedTile()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = GetRaycastHit();
            if (hit.collider != null)
            {
                ProcessRaycastHit(hit);
            }
            else
            {
                Debug.Log("No tile was clicked.");
            }
        }
    }

    private RaycastHit2D GetRaycastHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
    }

    private void ProcessRaycastHit(RaycastHit2D hit)
    {
        GameObject clickedTile = hit.collider.gameObject;
        Transform parentTransform = clickedTile.transform.parent;

        if (parentTransform != null)
        {
            string parentName = parentTransform.gameObject.name;

            if (parentName == "BarrierContainer" || parentName == "GroundContainer")
            {
                Debug.Log($"Raycast hit object in {parentName}");
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
