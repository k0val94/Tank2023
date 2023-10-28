using UnityEngine;
using System.Collections.Generic;
using System.IO;

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

    private List<string[]> mapLayers;
    private int groundSortingOrder = 0;
    private int barrierSortingOrder = 1;
    private Camera mainCamera;
    private MapLoader mapLoader;

    private void Start()
    {
        mainCamera = Camera.main;


        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "Game")
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
        else if (sceneName == "MapCreator")
        {
            mapLoader = GetComponent<MapLoader>();
            if (mapLoader == null)
            {
                Debug.LogError("MapLoader component not found!");
            }
            List<string[]> loadedMap = mapLoader.LoadMapFromFile("temp.map");
            BuildMap(loadedMap);

        }
        
    }

    public void BuildMap(List<string[]> mapDataLayers)
    {
        Debug.Log("Map generation started.");

        int width = mapDataLayers[0][0].Length;
        int height = mapDataLayers[0].Length;

        Vector3 mapCenter = new Vector3((width * MapData.Instance.tileSize / 100.0f) / 2, (height * MapData.Instance.tileSize / 100.0f) / 2, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * MapData.Instance.tileSize / 100.0f, (height - y - 1) * MapData.Instance.tileSize / 100.0f, 0);
                position -= mapCenter;

                char tileType = mapDataLayers[groundSortingOrder][y][x];

                switch (tileType)
                {
                    case 'D':
                        GameObject dirt = Instantiate(dirtPrefab, position, Quaternion.identity);
                        dirt.GetComponent<Renderer>().sortingOrder = groundSortingOrder;
                        dirt.transform.SetParent(groundContainer.transform);
                        break;
                    case 'Q':
                        GameObject quicksand = Instantiate(quicksandPrefab, position, Quaternion.identity);
                        quicksand.GetComponent<Renderer>().sortingOrder = groundSortingOrder;
                        quicksand.transform.SetParent(groundContainer.transform);
                        break;
                }
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * MapData.Instance.tileSize / 100.0f, (height - y - 1) * MapData.Instance.tileSize / 100.0f, 0);
                position -= mapCenter;

                char tileType = mapDataLayers[barrierSortingOrder][y][x];

                switch (tileType)
                {
                    case 'B':
                        GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity);
                        brick.GetComponent<Renderer>().sortingOrder = barrierSortingOrder;
                        brick.transform.SetParent(barrierContainer.transform);
                        break;
                    case 'S':
                        GameObject steel = Instantiate(steelPrefab, position, Quaternion.identity);
                        steel.GetComponent<Renderer>().sortingOrder = barrierSortingOrder;
                        steel.transform.SetParent(barrierContainer.transform);
                        break;
                    case 'W':
                        GameObject water = Instantiate(waterPrefab, position, Quaternion.identity);
                        water.GetComponent<Renderer>().sortingOrder = barrierSortingOrder;
                        water.transform.SetParent(barrierContainer.transform);
                        break;
                }
            }
        }

        Debug.Log("Map Build completed.");
    }

}