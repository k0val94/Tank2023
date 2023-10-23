using UnityEngine;
public class MapCleaner : MonoBehaviour
{
    public void CleanupMap()
    {
        string[] tagsToDelete = new string[] { "Dirt", "Brick", "Steel", "Water", "Quicksand" };

        foreach (string tag in tagsToDelete)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objectsWithTag)
            {
                Destroy(obj);
            }
        }
    }
}