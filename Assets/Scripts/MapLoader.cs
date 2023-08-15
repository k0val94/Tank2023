using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapLoader : MonoBehaviour
{
    public string mapFileName = "eight.map";

    public List<string[]> LoadMapFromFile()
    {
        string mapText = File.ReadAllText(Application.dataPath + "/Maps/" + mapFileName);
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
}