using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapGenerator : MonoBehaviour
{
    public GameObject brickPrefab;
    public GameObject playerPrefab;
    public GameObject dirtPrefab;
    public GameObject steelPrefab;
    private float tileSize = 64;

    private List<string[]> mapLayers;

    private float groundLevelZ = 0.0f;
    private float barrierLevelZ = -0.01f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        mapLayers = LoadMapFromFile("test.map");

        if (mapLayers != null && mapLayers.Count == 2)
        {
            GenerateMap(mapLayers);
        }
        else
        {
            Debug.LogError("Karte konnte nicht geladen werden. Überprüfen Sie die Datei und den Pfad.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button was clicked.");
            if (!PlayerManager.Instance.IsPlayerSpawned())
            {
                Debug.Log("Player has not spawned yet.");
                SpawnPlayerOnMouseClick();
            }
            else
            {
                Debug.Log("Player has already been spawned.");
            }
        }
    }

    void SpawnPlayerOnMouseClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            Debug.Log("Ray hit an object: " + hit.collider.name);
            if (!hit.collider.CompareTag("Brick"))
            {
                Debug.Log("Ray hit a valid spawn location.");
                PlayerManager.Instance.SpawnPlayer(hit.point);
            }
            else
            {
                Debug.Log("Ray hit an brick. Cannot spawn here.");
            }
        }
        else
        {
            Debug.Log("Ray did not hit any object.");
            PlayerManager.Instance.SpawnPlayer(ray.origin);
        }
    }


    private List<string[]> LoadMapFromFile(string fileName)
    {
        string mapText = File.ReadAllText(Application.dataPath + "/Maps/" + fileName);
        if (string.IsNullOrEmpty(mapText))
        {
            Debug.LogError("Map-Datei konnte nicht gefunden werden.");
            return null;
        }

        var layerData = mapText.Split(new[] { "---" }, System.StringSplitOptions.None);
        List<string[]> mapDataLayers = new List<string[]>();

        foreach (var layer in layerData)
        {
            mapDataLayers.Add(layer.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        return mapDataLayers;
    }

    void GenerateMap(List<string[]> mapDataLayers)
    {
        Debug.Log("Map-Generierung gestartet.");

        int width = mapDataLayers[0][0].Length;
        int height = mapDataLayers[0].Length;

        Vector3 mapCenter = new Vector3((width * tileSize / 100.0f) / 2, (height * tileSize / 100.0f) / 2, 0);

        // Check if brick and player objects are assigned
        if (brickPrefab == null || playerPrefab == null || dirtPrefab == null)
        {
            Debug.LogError("Einige GameObjects sind nicht zugewiesen.");
            return;
        }

        // Process Layer 1 (Dirt)
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

        // Process Layer 2 (Bricks, Steels)
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

        Debug.Log("Map-Generierung abgeschlossen.");
    }
}
