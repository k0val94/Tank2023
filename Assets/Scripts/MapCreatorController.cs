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
    private MapEditor mapEditor;

    [SerializeField] private TMP_InputField widthInputField;
    [SerializeField] private TMP_InputField heightInputField;
    [SerializeField] private TMP_InputField mapNameInput;
    [SerializeField] private Button generateMapButton;
    [SerializeField] private Button saveMapButton;
    [SerializeField] private ToggleGroup mapTypeToggleGroup;
    [SerializeField] private Toggle coastMapToggle;
    [SerializeField] private Toggle islandMapToggle;
    [SerializeField] private Toggle groundLayerToggle;
    [SerializeField] private Toggle barrierLayerToggle;

    

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
        
        mapEditor = GetComponent<MapEditor>();
        if (mapEditor == null)
        {
            Debug.LogError("MapEditor component not found!");
        }

        MapData.Instance.mapLayers = mapLoader.LoadMapFromFile("temp.map");

        groundLayerToggle.onValueChanged.AddListener(delegate { ToggleVisibility(mapBuilder.groundContainer, groundLayerToggle); });
        barrierLayerToggle.onValueChanged.AddListener(delegate { ToggleVisibility(mapBuilder.barrierContainer, barrierLayerToggle); });
        generateMapButton.onClick.AddListener(GenerateMap);
        saveMapButton.onClick.AddListener(SaveMap);
    }
    
    private void GenerateMap()
    {
        Debug.Log("Attempting to generate map.");
        int mapWidth, mapHeight;
        Toggle selectedToggle = null;
        
        foreach (Toggle toggle in mapTypeToggleGroup.ActiveToggles())
        {
            selectedToggle = toggle;
            break;
        }
        
        if (int.TryParse(widthInputField.text, out mapWidth) && int.TryParse(heightInputField.text, out mapHeight))
        {
            mapCleaner.CleanupMap();
            Debug.Log($"Generating map with dimensions: {mapWidth}x{mapHeight}");
            
            if (selectedToggle != null)
            {
                if (selectedToggle == coastMapToggle)
                {
                    Debug.Log($"coastMapToggle");
                    mapGenerator.GenerateRandomCoastMap(mapWidth, mapHeight);
                }
                else if (selectedToggle == islandMapToggle)
                {
                    Debug.Log($"islandMapToggle");
                    mapGenerator.GenerateRandomIslandMap(mapWidth, mapHeight);
                }
            }
            else
            {
                Debug.LogError("Bitte wählen Sie eine Kartenart aus.");
                return;
            }

            mapSaver.SaveMapToFile(mapGenerator.GetMapLayers(), "temp.map");
            List<string[]> loadedMap = mapLoader.LoadMapFromFile("temp.map");
            mapBuilder.BuildMap(loadedMap);
        }
        else
        {
            Debug.LogError("Ungültige Eingabe für Breite oder Höhe!");
        }
    }

    private void SaveMap() //with Button
    {
        Debug.Log("Attempting to save map.");
        string mapName = mapNameInput.text;

        if (string.IsNullOrEmpty(mapName))
        {
            Debug.LogError("Map name is empty! Please enter a valid map name.");
            return;
        }


        if (MapData.Instance.mapLayers == null)
        {
            Debug.LogError("Map layers are null! Make sure the map is generated properly.");
            return;
        }

        mapSaver.SaveMapToFile(MapData.Instance.mapLayers, mapName + ".map");
        Debug.Log($"Map saved as {mapName}.map");
    }

    public void BackToMainMenu()
    {
        Debug.Log("Back to Main Menu from Map Creator.");
        SceneManager.LoadScene("MainMenu");
    }

    private void ToggleVisibility(GameObject container, Toggle toggle)
    {
        if (container == null)
        {
            Debug.LogError("Container is null!");
            return;
        }
        container.SetActive(toggle.isOn);
    }
}