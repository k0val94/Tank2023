
using UnityEngine;
using System.Collections.Generic;
using System.IO;
public class MapSaver : MonoBehaviour
{
    public void SaveMapToFile(List<string[]> mapLayers, string fileName)
    {
        string mapText = string.Join("\n---\n", mapLayers.ConvertAll(layer => string.Join("\n", layer)));

        // Verwenden Sie Path.Combine, um den gewünschten Speicherort zu erstellen (StreamingAssets/Maps)
        string folderPath = Path.Combine(Application.streamingAssetsPath, "Maps");
        string path = Path.Combine(folderPath, fileName);

        // Erstellen Sie den Verzeichnispfad, falls er nicht vorhanden ist
        Directory.CreateDirectory(folderPath);

        File.WriteAllText(path, mapText);
        Debug.Log("Map saved to: " + path);
    }
}

