using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class RandomMapGenerator : MonoBehaviour
{

    [Header("Map Settings")]
    [SerializeField] private int mapWidth = 15;
    [SerializeField] private int mapHeight = 15;
    private List<string[]> mapLayers;
    private void Start()
    {
        //GenerateRandomCoastMap();
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

        // Generate unsymmetrical island barrier layer
        string[] barrierLayer = new string[mapHeight];

        int lastLeftWaterEdge = Random.Range(mapWidth / 4, 3 * mapWidth / 4);
        int lastRightWaterEdge = Random.Range(mapWidth / 4, 3 * mapWidth / 4);

        for (int i = 0; i < mapHeight; i++)
        {
            int leftOffset = Random.Range(-2, 3);
            int rightOffset = Random.Range(-2, 3);
            
            lastLeftWaterEdge = Mathf.Clamp(lastLeftWaterEdge + leftOffset, 2, 3 * mapWidth / 4);
            lastRightWaterEdge = Mathf.Clamp(lastRightWaterEdge + rightOffset, mapWidth / 4, mapWidth - 2);

            barrierLayer[i] = new string('W', lastLeftWaterEdge) + new string('.', lastRightWaterEdge - lastLeftWaterEdge) + new string('W', mapWidth - lastRightWaterEdge);
        }

        mapLayers.Add(barrierLayer);

        // Save the map to a file
        SaveMapToFile("random_map.map");
    }

    public void GenerateRandomCoastMap()
    {
        mapLayers = new List<string[]>();

        // Generate random ground layer
        string[] groundLayer = new string[mapHeight];
        for (int i = 0; i < mapHeight; i++)
        {
            groundLayer[i] = new string('D', mapWidth);
        }
        mapLayers.Add(groundLayer);

        // Generate unsymmetrical barrier layer with irregular water edge
        string[] barrierLayer = new string[mapHeight];
        int lastWaterEdgeStart = Random.Range(3, mapWidth - 3);
        for (int i = 0; i < mapHeight; i++)
        {
            int randomOffset = Random.Range(-2, 3); // This will move the water edge randomly by -2 to 2 units
            lastWaterEdgeStart = Mathf.Clamp(lastWaterEdgeStart + randomOffset, 3, mapWidth - 3); // Keep the edge within bounds

            barrierLayer[i] = new string('.', lastWaterEdgeStart) + new string('W', mapWidth - lastWaterEdgeStart);
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

    public List<string[]> GetMapLayers()
    {
        return mapLayers;
    }

}