using UnityEngine;
using System.Collections.Generic;
public class MapData : MonoBehaviour
{
    public static MapData Instance;
    public List<string[]> mapLayers; 

    private void Awake()
    {
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