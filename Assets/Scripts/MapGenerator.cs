using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    private List<string[]> mapLayers;
    private const char Quicksand = 'Q';
    private const char Dirt = 'D';
    private const char Water = 'W';
    private const char Space = '.';

    public void GenerateRandomIslandMap(int mapWidth, int mapHeight)
    {
        mapLayers = new List<string[]>();

        // Generate random island map
        string[] groundLayer = GenerateIslandMap(mapWidth, mapHeight);
        mapLayers.Add(groundLayer);

        // Generate barrier layer
        string[] barrierLayer = GenerateBarrierLayer(mapWidth, mapHeight);
        mapLayers.Add(barrierLayer);
    }

    public void GenerateRandomCoastMap(int mapWidth, int mapHeight)
    {
        mapLayers = new List<string[]>();

        // Generate random coast map
        string[] coastLayer = GenerateCoastMap(mapWidth, mapHeight);
        mapLayers.Add(coastLayer);

        // Generate barrier layer
        string[] barrierLayer = GenerateBarrierLayer(mapWidth, mapHeight);
        mapLayers.Add(barrierLayer);
    }

    private string[] GenerateIslandMap(int mapWidth, int mapHeight)
    {
        string[] mapLayer = new string[mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            char[] row = new char[mapWidth];

            for (int x = 0; x < mapWidth; x++)
            {
                if (IsCoast(x, y, mapWidth, mapHeight))
                {
                    row[x] = Quicksand; // Coast is quicksand
                }
                else if (IsLand(x, y, mapWidth, mapHeight))
                {
                    row[x] = Dirt; // Inside the island is dirt
                }
                else
                {
                    row[x] = Water; // Everything else is water
                }
            }

            mapLayer[y] = new string(row);
        }

        return mapLayer;
    }

    private string[] GenerateCoastMap(int mapWidth, int mapHeight)
    {
        string[] mapLayer = new string[mapHeight];
        float offsetX = Random.Range(0f, mapWidth);
        float offsetY = Random.Range(0f, mapHeight);

        for (int y = 0; y < mapHeight; y++)
        {
            char[] row = new char[mapWidth];

            for (int x = 0; x < mapWidth; x++)
            {
                float xCoord = (float)x / mapWidth + offsetX;
                float yCoord = (float)y / mapHeight + offsetY;
                float noise = Mathf.PerlinNoise(xCoord, yCoord);

                if (noise < 0.4f)
                {
                    row[x] = Water;
                }
                else if (noise < 0.6f)
                {
                    row[x] = Quicksand;
                }
                else
                {
                    row[x] = Dirt;
                }
            }

            mapLayer[y] = new string(row);
        }

        return mapLayer;
    }

    private string[] GenerateBarrierLayer(int mapWidth, int mapHeight)
    {
        string[] barrierLayer = new string[mapHeight];
        for (int i = 0; i < mapHeight; i++)
        {
            barrierLayer[i] = new string(Space, mapWidth);
        }
        return barrierLayer;
    }

    private bool IsCoast(int x, int y, int mapWidth, int mapHeight)
    {
        // Define the coast condition here
        int borderWidth = 2;
        return IsLand(x, y, mapWidth, mapHeight) &&
               (!IsLand(x - borderWidth, y, mapWidth, mapHeight) ||
                !IsLand(x + borderWidth, y, mapWidth, mapHeight) ||
                !IsLand(x, y - borderWidth, mapWidth, mapHeight) ||
                !IsLand(x, y + borderWidth, mapWidth, mapHeight));
    }

    private bool IsLand(int x, int y, int mapWidth, int mapHeight)
    {
        // Define the island shape here
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;
        int radius = Mathf.Min(mapWidth, mapHeight) / 4;

        return (x - centerX) * (x - centerX) + (y - centerY) * (y - centerY) <= radius * radius;
    }

    public List<string[]> GetMapLayers()
    {
        return mapLayers;
    }
}
