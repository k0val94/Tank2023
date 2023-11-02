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

        for (int i = 0; i < mapDataLayers.Count; i++)
        {
            CreateTilesForLayer(mapDataLayers, i);
        }

        Debug.Log("Map Build completed.");
    }

    private void CreateTilesForLayer(List<string[]> mapDataLayers, int layerIndex)
    {
        int width = mapDataLayers[0][0].Length;
        int height = mapDataLayers[0].Length;

        Vector3 mapCenter = new Vector3((width * MapData.Instance.tileSize / 100.0f) / 2, (height * MapData.Instance.tileSize / 100.0f) / 2, 0);

        GameObject container = layerIndex == 0 ? groundContainer : barrierContainer;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * MapData.Instance.tileSize / 100.0f, (height - y - 1) * MapData.Instance.tileSize / 100.0f, 0);
                position -= mapCenter;

                char tileTypeChar = mapDataLayers[layerIndex][y][x];
                TileType tileType = (TileType)tileTypeChar;

                CreateTile(tileType, position, layerIndex, container);
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
}
