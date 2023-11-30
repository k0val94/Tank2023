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
    private bool[,] walkableGrid;

    private enum TileType
    {
        Dirt = 'D',
        Quicksand = 'Q',
        Brick = 'B',
        Steel = 'S',
        Water = 'W'
    }

    private void Start()
    {
        mainCamera = Camera.main;
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

    private bool IsGameScene(string sceneName) => sceneName == "Game";
    private bool IsMapCreatorScene(string sceneName) => sceneName == "MapCreator";

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
        walkableGrid = new bool[MapData.Instance.width, MapData.Instance.height];

        MapData.Instance.mapCenter = new Vector3(
            (MapData.Instance.width * MapData.Instance.tileSize / 100.0f) / 2, 
            (MapData.Instance.height * MapData.Instance.tileSize / 100.0f) / 2, 
            0);

        for (int i = 0; i < mapDataLayers.Count; i++)
        {
            GameObject container = i == 0 ? groundContainer : barrierContainer;
            CreateTilesForLayer(mapDataLayers, i, container);
        }

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
                TileType tileType = (TileType)tileTypeChar;

                CreateTile(tileType, position, layerIndex, container);

                bool isGroundWalkable = IsTileWalkable(mapDataLayers[0][y][x]);
                bool isBarrierWalkable = layerIndex == 1 ? IsTileWalkable(tileTypeChar) : true;
                walkableGrid[x, y] = isGroundWalkable && isBarrierWalkable;
            }
        }
    }

    private void CreateTile(TileType tileType, Vector3 position, int sortingOrder, GameObject container)
    {
        GameObject prefab = tileType switch
        {
            TileType.Dirt => dirtPrefab,
            TileType.Quicksand => quicksandPrefab,
            TileType.Brick => brickPrefab,
            TileType.Steel => steelPrefab,
            TileType.Water => waterPrefab,
            _ => null
        };

        if (prefab != null)
        {
            GameObject tile = Instantiate(prefab, position, Quaternion.identity);
            tile.GetComponent<Renderer>().sortingOrder = sortingOrder;
            tile.transform.SetParent(container.transform);
        }
    }

    private bool IsTileWalkable(char tileTypeChar)
    {
        return tileTypeChar != (char)TileType.Brick &&
               tileTypeChar != (char)TileType.Steel &&
               tileTypeChar != (char)TileType.Water; // Include water as unwalkable
    }

    public bool[,] GetWalkableGrid()
    {
        return walkableGrid;
    }

    void OnDrawGizmos()
    {
        if (walkableGrid == null)
            return;

        for (int x = 0; x < MapData.Instance.width; x++)
        {
            for (int y = 0; y < MapData.Instance.height; y++)
            {
                Vector3 pos = new Vector3(
                    x * MapData.Instance.tileSize / 100.0f, 
                    (MapData.Instance.height - y - 1) * MapData.Instance.tileSize / 100.0f, 
                    0) - MapData.Instance.mapCenter;

                // Choose color based on walkability and make it semi-transparent
                Color gizmoColor = walkableGrid[x, y] ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
                Gizmos.color = gizmoColor;

                // Draw a wireframe cube for better visibility over the map
                Vector3 cubeSize = new Vector3(MapData.Instance.tileSize / 100.0f, MapData.Instance.tileSize / 100.0f, 1f) * 0.9f;
                Gizmos.DrawWireCube(pos, cubeSize);
            }
        }
    }
}
