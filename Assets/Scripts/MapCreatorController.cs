using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapCreatorController : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private MapLoader mapLoader;
    private MapBuilder mapBuilder;
    private MapCleaner mapCleaner; 
    private MapSaver mapSaver; 

    [SerializeField] private GameObject MapCreatorUI;
    [SerializeField] private TMP_InputField widthInputField;
    [SerializeField] private TMP_InputField heightInputField;
    [SerializeField] private TMP_InputField mapNameInput;
    [SerializeField] private Button generateMapButton;
    [SerializeField] private Button saveMapButton;

    private void Start()
    {
        Debug.Log("MapCreatorController Started.");

        mapGenerator = GetComponent<MapGenerator>();
        if (mapGenerator == null)
        {
            Debug.LogError("MapGenerator component not found!");
        }

        mapLoader = GetComponent<MapLoader>();
        if (mapLoader == null)
        {
            Debug.LogError("MapLoader component not found!");
        }

        mapBuilder = GetComponent<MapBuilder>();
        if (mapBuilder == null)
        {
            Debug.LogError("MapBuilder component not found!");
        }

        mapCleaner = GetComponent<MapCleaner>();
        if (mapCleaner == null)
        {
            Debug.LogError("MapCleaner component not found!");
        }

        mapSaver = GetComponent<MapSaver>();
        if (mapSaver == null)
        {
            Debug.LogError("MapSaver component not found!");
        }

        MapCreatorUI.SetActive(true);
        generateMapButton.onClick.AddListener(GenerateMap);
        saveMapButton.onClick.AddListener(SaveMap);
    }
    
    private void GenerateMap()
    {
        Debug.Log("Attempting to generate map.");
        int mapWidth, mapHeight;
        
        if (int.TryParse(widthInputField.text, out mapWidth) && int.TryParse(heightInputField.text, out mapHeight))
        {
            mapCleaner.CleanupMap();
            Debug.Log($"Generating map with dimensions: {mapWidth}x{mapHeight}");
            mapGenerator.GenerateRandomMap(mapWidth, mapHeight);
            mapSaver.SaveMapToFile(mapGenerator.GetMapLayers(), "temp.map");
            List<string[]> loadedMap = mapLoader.LoadMapFromFile("temp.map");
            mapBuilder.BuildMap(loadedMap);
        }
        else
        {
            Debug.LogError("Ungültige Eingabe für Breite oder Höhe!");
        }
    }

    private void SaveMap()
    {
        Debug.Log("Attempting to save map.");
        string mapName = mapNameInput.text;

        if (string.IsNullOrEmpty(mapName))
        {
            Debug.LogError("Map name is empty! Please enter a valid map name.");
            return;
        }

        mapSaver.SaveMapToFile(mapGenerator.GetMapLayers(), mapName + ".map");
        Debug.Log($"Map saved as {mapName}.map");
    }

    public void BackToMainMenu()
    {
        Debug.Log("Back to Main Menu from Map Creator.");
        SceneManager.LoadScene("MainMenu");
    }
}