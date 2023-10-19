using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public List<string[]> LoadMapFromFile(string fileName)
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

        return mapDataLayers;
    }
}