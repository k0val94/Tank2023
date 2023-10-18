using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RandomMapGenerator : MonoBehaviour
{

    [Header("Map Settings")]
    [SerializeField] private int mapWidth = 12;
    [SerializeField] private int mapHeight = 6;
    private List<string[]> mapLayers;
    private void Start()
    {
        GenerateRandomMap();
    }

    public void GenerateRandomMap()
    {
        mapLayers = new List<string[]>();

        // Generate random ground layer
        string[] groundLayer = new string[mapHeight];
        for (int i = 0; i < mapHeight; i++)
        {
            groundLayer[i] = new string('D', mapWidth);
        }
        mapLayers.Add(groundLayer);

        // Generate random barrier layer with irregular water edge
        string[] barrierLayer = new string[mapHeight];
        for (int i = 0; i < mapHeight; i++)
        {
            if (i < 2 || i >= mapHeight - 2)
            {
                // Water at the top and bottom
                barrierLayer[i] = new string('W', mapWidth);
            }
            else
            {
                // Random terrain with water edge
                int waterEdgeStart = Random.Range(2, mapWidth - 2);
                barrierLayer[i] = new string('D', waterEdgeStart) + new string('W', mapWidth - waterEdgeStart);
            }
        }
        mapLayers.Add(barrierLayer);

        // Save the map to a file
        SaveMapToFile("random_map.map");
    }

    private void SaveMapToFile(string fileName)
    {
        string mapText = string.Join("\n---\n", mapLayers.ConvertAll(layer => string.Join("\n", layer)));

        // Verwenden Sie Path.Combine, um den gewünschten Speicherort zu erstellen (StreamingAssets/Maps)
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Maps");
        string path = Path.Combine(folderPath, fileName);

        // Erstellen Sie den Verzeichnispfad, falls er nicht vorhanden ist
        Directory.CreateDirectory(folderPath);

        File.WriteAllText(path, mapText);
        Debug.Log("Random map saved to: " + path);
    }

}