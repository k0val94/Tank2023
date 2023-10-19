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

    [Header("Other")]
    [SerializeField] private RandomMapGenerator randomMapGenerator;

    private List<string[]> mapLayers;
    private float tileSize = 64;
    private int groundSortingOrder = 0;
    private int barrierSortingOrder = 1;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        if (MapData.Instance.mapLayers != null && MapData.Instance.mapLayers.Count == 2)
        {
            BuildMap(MapData.Instance.mapLayers);
        }
        else
        {
            Debug.LogError("Map data not found in MapData.");
        }
    }

    void BuildMap(List<string[]> mapDataLayers)
    {
        Debug.Log("Map generation started.");

        int width = mapDataLayers[0][0].Length;
        int height = mapDataLayers[0].Length;

        Vector3 mapCenter = new Vector3((width * tileSize / 100.0f) / 2, (height * tileSize / 100.0f) / 2, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * tileSize / 100.0f, (height - y - 1) * tileSize / 100.0f, 0);
                position -= mapCenter;

                char tileType = mapDataLayers[groundSortingOrder][y][x];

                switch (tileType)
                {
                    case 'D':
                        GameObject dirt = Instantiate(dirtPrefab, position, Quaternion.identity);
                        dirt.GetComponent<Renderer>().sortingOrder = groundSortingOrder;
                        break;
                    case 'Q':
                        GameObject quicksand = Instantiate(quicksandPrefab, position, Quaternion.identity);
                        quicksand.GetComponent<Renderer>().sortingOrder = groundSortingOrder;
                        break;
                }
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * tileSize / 100.0f, (height - y - 1) * tileSize / 100.0f, 0);
                position -= mapCenter;

                char tileType = mapDataLayers[barrierSortingOrder][y][x];

                switch (tileType)
                {
                    case 'B':
                        GameObject brick = Instantiate(brickPrefab, position, Quaternion.identity);
                        brick.GetComponent<Renderer>().sortingOrder = barrierSortingOrder;
                        break;
                    case 'S':
                        GameObject steel = Instantiate(steelPrefab, position, Quaternion.identity);
                        steel.GetComponent<Renderer>().sortingOrder = barrierSortingOrder;
                        break;
                    case 'W':
                        GameObject water = Instantiate(waterPrefab, position, Quaternion.identity);
                        water.GetComponent<Renderer>().sortingOrder = barrierSortingOrder;
                        break;
                }
            }
        }

        Debug.Log("Map Build completed.");
    }

    public void BuildMapFromGenerator()
    {
        if(randomMapGenerator == null)
        {
            Debug.LogError("RandomMapGenerator reference is not set.");
            return;
        }

        mapLayers = randomMapGenerator.GetMapLayers();

        if (mapLayers != null && mapLayers.Count >= 2)
        {
            BuildMap(mapLayers);
        }
        else
        {
            Debug.LogError("Map could not be generated. Check the generation code.");
        }
    }
}