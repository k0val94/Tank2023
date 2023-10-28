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
        UpdateMapData(oldTilePosition, parentContainer.name, selectedTilePrefab.name[0]);
    }

    private void UpdateMapData(Vector3 position, string layerName, char newTileType)
    {
        Vector3 mapCenter = new Vector3((MapData.Instance.width * MapData.Instance.tileSize / 100.0f) / 2, (MapData.Instance.height * MapData.Instance.tileSize / 100.0f) / 2, 0);
        int x = Mathf.FloorToInt((position.x + mapCenter.x) / (MapData.Instance.tileSize / 100.0f));
        int y = MapData.Instance.height - 1 - Mathf.FloorToInt((position.y + mapCenter.y) / (MapData.Instance.tileSize / 100.0f));
        
        int layerIndex = (layerName == "GroundContainer") ? 0 : 1; // Assumes Ground is at 0, Barrier at 1

        if (y < 0 || y >= MapData.Instance.height || x < 0 || x >= MapData.Instance.width)
        {
            Debug.LogWarning($"Position out of map bounds: {position}");
            return;
        }

        string oldRow = MapData.Instance.mapLayers[layerIndex][y];
        char[] newRow = oldRow.ToCharArray();
        newRow[x] = newTileType;
        MapData.Instance.mapLayers[layerIndex][y] = new string(newRow);
    }
}
