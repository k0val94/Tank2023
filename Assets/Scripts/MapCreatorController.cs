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
        mapLoader = GetComponent<MapLoader>();
        mapBuilder = GetComponent<MapBuilder>();
        mapCleaner = GetComponent<MapCleaner>();
        mapSaver = GetComponent<MapSaver>();
        mapEditor = GetComponent<MapEditor>();

        MapData.Instance.mapLayers = mapLoader.LoadMapFromFile("temp.map");
        MapData.Instance.width = MapData.Instance.mapLayers[0][0].Length;
        MapData.Instance.height = MapData.Instance.mapLayers[0].Length;

        groundLayerToggle.onValueChanged.AddListener(delegate { UpdateTileDropdown(); ToggleVisibility(mapBuilder.groundContainer, groundLayerToggle); });
        barrierLayerToggle.onValueChanged.AddListener(delegate { UpdateTileDropdown(); ToggleVisibility(mapBuilder.barrierContainer, barrierLayerToggle); });
        generateMapButton.onClick.AddListener(GenerateMap);
        saveMapButton.onClick.AddListener(SaveMap);
        
        UpdateTileDropdown();
    }

    private void UpdateTileDropdown()
    {
        if (mapEditor == null) return;

        if (groundLayerToggle.isOn)
        {
            mapEditor.SetDropdownOptions(new string[] { "Dirt", "Quicksand", "Water" });
        }
        else if (barrierLayerToggle.isOn)
        {
            mapEditor.SetDropdownOptions(new string[] { "Steel", "Brick" });
        }
        else
        {
            mapEditor.ClearDropdownOptions();
        }
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
            MapData.Instance.mapLayers = mapLoader.LoadMapFromFile("temp.map");
            MapData.Instance.height = mapHeight;
            MapData.Instance.width = mapWidth;
            mapBuilder.BuildMap(MapData.Instance.mapLayers);
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