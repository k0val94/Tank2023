using UnityEngine;
using System.Collections.Generic;

public class MapData : MonoBehaviour
{
    public static MapData Instance;
    public List<string[]> mapLayers;
    
    public float tileSize { get; private set; } = 64f;
    public int width { get; set; }
    public int height { get; set; }

    private void Awake()
    {
        // Existing Singleton Logic
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
