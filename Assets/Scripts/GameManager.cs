using UnityEngine;

public class GameManager : MonoBehaviour
{
//    public EnemyTankManager enemyTankManager;
    public MapGenerator mapGenerator;

    private void Start()
    {
        if(mapGenerator != null)
        {
            mapGenerator.Generate();
        }
        else
        {
            Debug.LogError("Map Generator is not assigned!");
        }
      /*  if(enemyTankManager != null)
        {
            enemyTankManager.StartSpawn();
        }
        else
        {
            Debug.LogError("Enemy Tank Manager is not assigned!");
        }*/
    }
}