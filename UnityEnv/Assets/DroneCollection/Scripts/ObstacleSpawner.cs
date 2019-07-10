using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject obstaclePrefab;
    [SerializeField]
    private int amount = 500;

    private void Start() 
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(obstaclePrefab, transform);
        }
    }
}