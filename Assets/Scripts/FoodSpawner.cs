using UnityEngine;
using GameRPG.Systems;

public sealed class FoodSpawner : MonoBehaviour, ISpawnReceiver
{
    [SerializeField] private GameObject[] foodPrefabs;

    public bool TrySpawn(in SpawnRequest request, out GameObject spawnedInstance)
    {
        spawnedInstance = null;
        if (request.Kind != SpawnKind.Food || foodPrefabs == null || foodPrefabs.Length == 0)
        {
            return false;
        }

        int index = Random.Range(0, foodPrefabs.Length);
        GameObject prefab = foodPrefabs[index];
        if (prefab == null)
        {
            return false;
        }

        spawnedInstance = Instantiate(prefab, request.Position, Quaternion.identity);
        return spawnedInstance != null;
    }
}
