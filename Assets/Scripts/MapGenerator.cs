using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapGenerator : MonoBehaviour
{
    public GameObject brickPrefab;
    public GameObject playerPrefab;
    public GameObject dirtPrefab;
    private float tileSize = 64;

    private List<string[]> mapLayers;

    private float groundLevelZ = 0.0f;
    private float elevatedLevelZ = -0.01f;

    void Start()
    {
        mapLayers = LoadMapFromFile("eight.map");

        if (mapLayers != null && mapLayers.Count == 2)
        {
            int width = mapLayers[0][0].Length;
            int height = mapLayers[0].Length;

            AdjustCameraSize(width, height);
            GenerateMap(mapLayers);
        }
        else
        {
            Debug.LogError("Karte konnte nicht geladen werden. Überprüfen Sie die Datei und den Pfad.");
        }
    }

    void AdjustCameraSize(int mapWidth, int mapHeight)
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = (float)mapWidth / (float)mapHeight;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = (float)mapHeight * tileSize / 200.0f;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = (float)mapHeight * tileSize / 200.0f * differenceInSize;
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

        // Process Layer 2 (Bricks, Player)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * tileSize / 100.0f, (height - y - 1) * tileSize / 100.0f, elevatedLevelZ);
                position -= mapCenter;

                char tileType = mapDataLayers[0][y][x];

                switch (tileType)
                {
                    case 'X':
                        Instantiate(brickPrefab, position, Quaternion.identity);
                        break;
                    case '1':
                        Instantiate(playerPrefab, position, Quaternion.identity);
                        break;
                    default:
                        break;
                }
            }
        }

        Debug.Log("Map-Generierung abgeschlossen.");
    }
}
