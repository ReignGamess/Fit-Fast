using UnityEngine;
using GameRPG.Systems;

public sealed class MonsterSpawner : MonoBehaviour, ISpawnReceiver
{
    //D10 görevi burada yapılmıştır
    [SerializeField] private GameObject[] enemyPrefabs;

    public bool TrySpawn(in SpawnRequest request, out GameObject spawnedInstance)
    {
        spawnedInstance = null;
        if (request.Kind != SpawnKind.Enemy || enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            return false;
        }

        int index = Random.Range(0, enemyPrefabs.Length);
        GameObject prefab = enemyPrefabs[index];
        if (prefab == null)
        {
            return false;
        }

        spawnedInstance = Instantiate(prefab, request.Position, Quaternion.identity);
        return spawnedInstance != null;
    }
}