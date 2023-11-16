using UnityEngine;
using System.Collections.Generic;

public class SpriteHolder : MonoBehaviour
{
    [SerializeField] private List<Sprite> spritesList = new List<Sprite>();

    public List<Sprite> GetSpritesList()
    {
        return spritesList;
    }

}