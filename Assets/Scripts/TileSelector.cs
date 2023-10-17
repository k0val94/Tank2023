using UnityEngine;

public class TileSelector : MonoBehaviour
{
    public static GameObject currentTile;

    public GameObject thisTile;

    public void SetTile()
    {
        currentTile = thisTile;
    }
}
