using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MapGenerator : MonoBehaviour
{
    [Header("Tile Prefabs")]
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private GameObject dirtPrefab;
    [SerializeField] private GameObject steelPrefab;

    private List<string[]> mapLayers;
    private float tileSize = 64;
    private float groundLevelZ = 0.0f;
    private float barrierLevelZ = -0.01f;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        mapLayers = LoadMapFromFile("test.map");

        if (mapLayers != null && mapLayers.Count == 2)
        {
            GenerateMap(mapLayers);
        }
        else
        {
            Debug.LogError("Map could not be loaded. Check the file and path.");
        }
    }

    private List<string[]> LoadMapFromFile(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Maps", fileName);
        
        Debug.Log("Looking for map at: " + path);

        if (!File.Exists(path))
        {
            Debug.LogError("Map file not found at: " + path);
            return null;
        }

        string mapText = File.ReadAllText(path);

        var layerData = mapText.Split(new[] { "---" }, System.StringSplitOptions.None);
        List<string[]> mapDataLayers = new List<string[]>();

        foreach (var layer in layerData)
        {
            mapDataLayers.Add(layer.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        foreach (var layer in mapDataLayers)
        {
            foreach (var line in layer)
            {
                Debug.Log(line);
            }
        }

        return mapDataLayers;
    }

    void GenerateMap(List<string[]> mapDataLayers)
    {
        Debug.Log("Map generation started.");

        int width = mapDataLayers[0][0].Length;
        int height = mapDataLayers[0].Length;

        Vector3 mapCenter = new Vector3((width * tileSize / 100.0f) / 2, (height * tileSize / 100.0f) / 2, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * tileSize / 100.0f, (height - y - 1) * tileSize / 100.0f, groundLevelZ);
                position -= mapCenter;

                char tileType = mapDataLayers[1][y][x];

                if (tileType == 'D')
                {
                    Instantiate(dirtPrefab, position, Quaternion.identity);
                }
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * tileSize / 100.0f, (height - y - 1) * tileSize / 100.0f, barrierLevelZ);
                position -= mapCenter;

                char tileType = mapDataLayers[0][y][x];

                switch (tileType)
                {
                    case 'B':
                        Instantiate(brickPrefab, position, Quaternion.identity);
                        break;
                    case 'S':
                        Instantiate(steelPrefab, position, Quaternion.identity);
                        break;
                }
            }
        }

        Debug.Log("Map generation completed.");
    }
}
