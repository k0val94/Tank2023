using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapCreatorController : MonoBehaviour
{
    private RandomMapGenerator mapGenerator;
    private MapLoader mapLoader;
    private MapBuilder mapBuilder;
    private MapCleaner mapCleaner; 
    private MapSaver mapSaver; 

    [SerializeField] private GameObject MapCreatorUI;
    [SerializeField] private TMP_InputField widthInputField;
    [SerializeField] private TMP_InputField heightInputField;
    [SerializeField] private Button generateMapButton;

    private void Start()
    {
        Debug.Log("MapCreatorController Started.");

        mapGenerator = GetComponent<RandomMapGenerator>();
        if (mapGenerator == null)
        {
            Debug.LogError("RandomMapGenerator component not found!");
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
    }

    public void GenerateMap()
    {
        Debug.Log("Attempting to generate map.");
        int mapWidth, mapHeight;
        List<string[]> loadedMap = null;
        mapCleaner.CleanupMap();

        if (int.TryParse(widthInputField.text, out mapWidth) && int.TryParse(heightInputField.text, out mapHeight))
        {
            Debug.Log($"Generating map with dimensions: {mapWidth}x{mapHeight}");
            mapGenerator.GenerateRandomMap(mapWidth, mapHeight);
            mapSaver.SaveMapToFile(mapGenerator.GetMapLayers(), "temp.map");
            loadedMap = mapLoader.LoadMapFromFile("temp.map");
            mapBuilder.BuildMap(loadedMap);
        }
        else
        {
            Debug.LogError("Ungültige Eingabe für Breite oder Höhe!");
        }
    }

    public void BackToMainMenu()
    {
        Debug.Log("Back to Main Menu from Map Creator.");
        SceneManager.LoadScene("MainMenu");
    }
}