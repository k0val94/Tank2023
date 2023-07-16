using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapGenerator : MonoBehaviour
{
    public GameObject brickPrefab;
    public GameObject playerPrefab;
    private float tileSize;
    private string[] mapData;

    void Start()
    {
        // Laden der Kartenstruktur aus der Datei
        mapData = LoadMapFromFile("frame.map");
        
        if (mapData != null)
        {
            int width = mapData[0].Length;
            int height = mapData.Length;

            // Kachelgröße anpassen, um die Karte in die Kameraansicht zu passen
            float cameraHeight = Camera.main.orthographicSize * 2;
            float cameraWidth = cameraHeight * Camera.main.aspect;
            tileSize = Mathf.Min(cameraHeight / height, cameraWidth / width) * 100;

            AdjustCameraSize(width, height);
            GenerateMap(mapData);
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
    private string[] LoadMapFromFile(string fileName)
    {
        string mapText = File.ReadAllText(Application.dataPath + "/Maps/" + fileName);
        if (string.IsNullOrEmpty(mapText))
        {
            Debug.LogError("Map-Datei konnte nicht gefunden werden.");
            return null;
        }
        return mapText.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    void GenerateMap(string[] mapData)
    {
        Debug.Log("Map-Generierung gestartet.");

        int width = mapData[0].Length;
        int height = mapData.Length;

        // Berechnung des Mittelpunkts der Karte
        Vector3 mapCenter = new Vector3((width * tileSize / 100.0f) / 2, (height * tileSize / 100.0f) / 2, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x * tileSize / 100.0f, (height - y - 1) * tileSize / 100.0f, 0);
                // Zentrieren der Karte, indem der Mittelpunkt von der Position abgezogen wird
                position -= mapCenter;

                char tileType = mapData[y][x];

                switch (tileType)
                {
                    case 'X':
                        Instantiate(brickPrefab, position, Quaternion.identity);
                        Debug.LogFormat("Brick erstellt bei: x = {0}, y = {1}", x, y);
                        break;
                    case '1':
                        Instantiate(playerPrefab, position, Quaternion.identity);
                        Debug.LogFormat("Spieler 1 erstellt bei: x = {0}, y = {1}", x, y);
                        break;
                    default:
                        break;
                }
            }
        }

        Debug.Log("Map-Generierung abgeschlossen.");
    }
}
