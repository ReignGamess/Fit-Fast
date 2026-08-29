using System.Collections;
using GameRPG.Systems;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;

namespace GameRPG.Tests.PlayMode
{
    public sealed class MapSpawnSchedulerPlayModeTests
    {
        private sealed class FakeReceiver : MonoBehaviour, ISpawnReceiver
        {
            public int SpawnCount { get; private set; }
            public Vector3 LastPosition { get; private set; }
            public GameObject LastSpawned { get; private set; }

            public bool TrySpawn(in SpawnRequest request, out GameObject spawnedInstance)
            {
                spawnedInstance = new GameObject("Spawned");
                spawnedInstance.transform.position = request.Position;
                LastSpawned = spawnedInstance;
                LastPosition = request.Position;
                SpawnCount++;
                return true;
            }
        }

        [UnityTest]
        public IEnumerator SchedulerUsesWalkableCellsAndHonorsLimits()
        {
            GameObject root = new("Grid", typeof(Grid), typeof(TilemapMap));
            GameObject groundObject = new("Ground", typeof(Tilemap), typeof(TilemapRenderer));
            GameObject obstaclesObject = new("Obstacles", typeof(Tilemap), typeof(TilemapRenderer));
            groundObject.transform.SetParent(root.transform);
            obstaclesObject.transform.SetParent(root.transform);
            Tilemap ground = groundObject.GetComponent<Tilemap>();
            Tilemap obstacles = obstaclesObject.GetComponent<Tilemap>();
            Tile groundTile = ScriptableObject.CreateInstance<Tile>();
            Tile obstacleTile = ScriptableObject.CreateInstance<Tile>();
            ground.SetTile(Vector3Int.zero, groundTile);
            ground.SetTile(Vector3Int.right, groundTile);
            obstacles.SetTile(Vector3Int.right, obstacleTile);
            TilemapMap map = root.GetComponent<TilemapMap>();
            map.Configure(root.GetComponent<Grid>(), ground, obstacles);

            GameObject areaObject = new("Area", typeof(MapSpawnArea));
            MapSpawnArea area = areaObject.GetComponent<MapSpawnArea>();
            area.Configure(map, Vector2Int.zero, new Vector2Int(2, 1), 4);
            GameObject receiverObject = new("Receiver", typeof(FakeReceiver));
            FakeReceiver receiver = receiverObject.GetComponent<FakeReceiver>();
            GameObject schedulerObject = new("Scheduler", typeof(MapSpawnScheduler));
            MapSpawnScheduler scheduler = schedulerObject.GetComponent<MapSpawnScheduler>();
            SpawnChannel channel = new();
            channel.Configure("food", SpawnKind.Food, true, area, receiver, 0f, 1, 0.02f, false, Vector2.zero, 1, 2);
            scheduler.Configure(map, new[] { channel });

            yield return null;
            Assert.That(receiver.SpawnCount, Is.EqualTo(1));
            Assert.That(map.IsWalkable(receiver.LastPosition), Is.True);

            scheduler.NotifyReleased("food", receiver.LastSpawned);
            Object.Destroy(receiver.LastSpawned);
            yield return new WaitForSeconds(0.03f);
            yield return null;
            Assert.That(receiver.SpawnCount, Is.EqualTo(2));

            scheduler.NotifyReleased("food", receiver.LastSpawned);
            Object.Destroy(receiver.LastSpawned);
            yield return new WaitForSeconds(0.03f);
            yield return null;
            Assert.That(receiver.SpawnCount, Is.EqualTo(2));

            Object.Destroy(schedulerObject);
            Object.Destroy(receiverObject);
            Object.Destroy(areaObject);
            Object.Destroy(root);
            Object.Destroy(groundTile);
            Object.Destroy(obstacleTile);
        }
    }
}
