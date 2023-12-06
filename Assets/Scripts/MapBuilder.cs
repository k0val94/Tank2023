using UnityEngine;
using System.Collections.Generic;

public class MapBuilder : MonoBehaviour
{
    [Header("Tile Prefabs")]
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private GameObject dirtPrefab;
    [SerializeField] private GameObject steelPrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject quicksandPrefab;
    [SerializeField] public GameObject groundContainer;
    [SerializeField] public GameObject barrierContainer;

    private Camera mainCamera;
    private MapLoader mapLoader;
    private WalkableGridManager walkableGridManager;
    public bool isMapBuilt = false;

    private void Start()
    {
        mainCamera = Camera.main;
        walkableGridManager = GetComponent<WalkableGridManager>();
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (IsGameScene(sceneName))
        {
            BuildMapFromData();
        }
        else if (IsMapCreatorScene(sceneName))
        {
            BuildMapFromLoader();
        }
    }

    private bool IsGameScene(string sceneName)
    {
        return sceneName == "Game";
    }

    private bool IsMapCreatorScene(string sceneName)
    {
        return sceneName == "MapCreator";
    }

    private void BuildMapFromData()
    {
        if (MapData.Instance.mapLayers != null && MapData.Instance.mapLayers.Count == 2)
        {
            BuildMap(MapData.Instance.mapLayers);
        }
        else
        {
            Debug.LogWarning("Map data not found in MapData.");
        }
    }

    private void BuildMapFromLoader()
    {
        mapLoader = GetComponent<MapLoader>();
        if (mapLoader == null)
        {
            Debug.LogError("MapLoader component not found!");
            return;
        }

        List<string[]> loadedMap = mapLoader.LoadMapFromFile("temp.map");
        BuildMap(loadedMap);
    }

    public void BuildMap(List<string[]> mapDataLayers)
    {
        Debug.Log("Map generation started.");

        MapData.Instance.width = mapDataLayers[0][0].Length;
        MapData.Instance.height = mapDataLayers[0].Length;
        walkableGridManager.InitializeGrid(MapData.Instance.width, MapData.Instance.height);

        MapData.Instance.mapCenter = new Vector3(
            (MapData.Instance.width * MapData.Instance.tileSize / 100.0f) / 2, 
            (MapData.Instance.height * MapData.Instance.tileSize / 100.0f) / 2, 
            0);

        for (int i = 0; i < mapDataLayers.Count; i++)
        {
            GameObject container = i == 0 ? groundContainer : barrierContainer;
            CreateTilesForLayer(mapDataLayers, i, container);
        }
        
        isMapBuilt = true;
        Debug.Log("Map Build completed.");
    }

    private void CreateTilesForLayer(List<string[]> mapDataLayers, int layerIndex, GameObject container)
    {
        for (int y = 0; y < MapData.Instance.height; y++)
        {
            for (int x = 0; x < MapData.Instance.width; x++)
            {
                Vector3 position = new Vector3(
                    x * MapData.Instance.tileSize / 100.0f, 
                    (MapData.Instance.height - y - 1) * MapData.Instance.tileSize / 100.0f, 
                    0);
                position -= MapData.Instance.mapCenter;
                char tileTypeChar = mapDataLayers[layerIndex][y][x];
                WalkableGridManager.TileType tileType = (WalkableGridManager.TileType)tileTypeChar;

                CreateTile(tileType, position, layerIndex, container);

                bool isGroundWalkable = walkableGridManager.IsTileWalkable(mapDataLayers[0][y][x]);
                bool isBarrierWalkable = layerIndex == 1 ? walkableGridManager.IsTileWalkable(tileTypeChar) : true;
                walkableGridManager.SetTileWalkable(x, y, isGroundWalkable && isBarrierWalkable);
            }
        }
    }

    private void CreateTile(WalkableGridManager.TileType tileType, Vector3 position, int sortingOrder, GameObject container)
    {
        GameObject prefab = tileType switch
        {
            WalkableGridManager.TileType.Dirt => dirtPrefab,
            WalkableGridManager.TileType.Quicksand => quicksandPrefab,
            WalkableGridManager.TileType.Brick => brickPrefab,
            WalkableGridManager.TileType.Steel => steelPrefab,
            WalkableGridManager.TileType.Water => waterPrefab,
            _ => null
        };

        if (prefab != null)
        {
            GameObject tile = Instantiate(prefab, position, Quaternion.identity);
            tile.GetComponent<Renderer>().sortingOrder = sortingOrder;
            tile.transform.SetParent(container.transform);
        }
    }

}
