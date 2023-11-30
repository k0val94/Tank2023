using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    public TMP_Dropdown tileDropdown;
    public List<GameObject> tilePrefabs;
    private GameObject selectedTilePrefab;

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
        tileDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void InitializeTileSelection()
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

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

        if (!IsReplacementValid(oldTile.name, selectedTilePrefab.name))
        {
            Debug.LogError("Invalid tile replacement. Barrier tiles must be replaced with barrier tiles, and ground tiles with ground tiles.");
            return;
        }

        Destroy(oldTile);

        GameObject newTile = Instantiate(selectedTilePrefab, oldTilePosition, Quaternion.identity);
        newTile.GetComponent<SpriteRenderer>().sortingOrder = oldOrderInLayer;
        newTile.transform.SetParent(parentContainer.transform);
        UpdateMapData(oldTilePosition, parentContainer.name, selectedTilePrefab.name[0]);
    }

    private bool IsReplacementValid(string oldTileName, string newTileName)
    {
        return IsSameCategory(oldTileName, newTileName);
    }

    private bool IsSameCategory(string tileName1, string tileName2)
    {
        bool isTile1Barrier = IsBarrierTile(tileName1);
        bool isTile2Barrier = IsBarrierTile(tileName2);

        return isTile1Barrier == isTile2Barrier;
    }

    private bool IsBarrierTile(string tileName)
    {
        return tileName == "Steel" || tileName == "Brick";
    }

    private void UpdateMapData(Vector3 position, string layerName, char newTileType)
    {
        // Convert world position to array indices
        int xIndex = Mathf.RoundToInt((position.x + MapData.Instance.mapCenter.x) * 100.0f / MapData.Instance.tileSize);
        int yIndex = Mathf.RoundToInt((MapData.Instance.height - 1) - ((position.y + MapData.Instance.mapCenter.y) * 100.0f / MapData.Instance.tileSize));

        // Determine which layer to update based on the layerName
        int layerIndex = layerName == "BarrierContainer" ? 1 : 0;  // Assuming layer 0 is ground, layer 1 is barrier

        // Update the mapLayers array
        if (yIndex >= 0 && yIndex < MapData.Instance.height && xIndex >= 0 && xIndex < MapData.Instance.width)
        {
            MapData.Instance.mapLayers[layerIndex][yIndex] = MapData.Instance.mapLayers[layerIndex][yIndex].Remove(xIndex, 1).Insert(xIndex, newTileType.ToString());
        }
        else
        {
            Debug.LogError("Attempted to update map data with invalid indices.");
        }
    }

    public void SetDropdownOptions(string[] options)
    {
        tileDropdown.ClearOptions();
        tileDropdown.AddOptions(new List<string>(options));
    }

    public void ClearDropdownOptions()
    {
        tileDropdown.ClearOptions();
    }
}
