using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameRPG.Systems
{
    [Serializable]
    public sealed class SpawnChannel
    {
        [SerializeField] private string channelId = "spawn";
        [SerializeField] private SpawnKind kind;
        [SerializeField] private bool enabled = true;
        [SerializeField] private MapSpawnArea area;
        [SerializeField] private MonoBehaviour receiver;
        [SerializeField] private float initialDelay;
        [SerializeField] private int initialBurst;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private bool useRandomInterval;
        [SerializeField] private Vector2 randomIntervalRange = new(2f, 5f);
        [SerializeField] private int maximumActive = 1;
        [SerializeField] private int totalLimit;

        private readonly List<GameObject> activeInstances = new();
        private ISpawnReceiver spawnReceiver;
        private float nextSpawnTime;
        private int remainingInitialBurst;
        private int totalSpawned;

        public string ChannelId => channelId;
        public int ActiveCount => activeInstances.Count;
        public int TotalSpawned => totalSpawned;

        public void Configure(
            string id,
            SpawnKind spawnKind,
            bool isEnabled,
            MapSpawnArea spawnArea,
            MonoBehaviour spawnReceiverComponent,
            float delay,
            int burst,
            float interval,
            bool randomizeInterval,
            Vector2 intervalRange,
            int activeLimit,
            int productionLimit)
        {
            channelId = id;
            kind = spawnKind;
            enabled = isEnabled;
            area = spawnArea;
            receiver = spawnReceiverComponent;
            initialDelay = Mathf.Max(0f, delay);
            initialBurst = Mathf.Max(0, burst);
            spawnInterval = Mathf.Max(0.01f, interval);
            useRandomInterval = randomizeInterval;
            randomIntervalRange = intervalRange;
            maximumActive = Mathf.Max(0, activeLimit);
            totalLimit = Mathf.Max(0, productionLimit);
        }

        public void ResetRuntime(float currentTime)
        {
            activeInstances.Clear();
            spawnReceiver = receiver as ISpawnReceiver;
            nextSpawnTime = currentTime + Mathf.Max(0f, initialDelay);
            remainingInitialBurst = Mathf.Max(0, initialBurst);
            totalSpawned = 0;
        }

        public void Tick(IMapQuery mapQuery, float currentTime)
        {
            PruneInactiveInstances();
            if (!enabled || area == null || spawnReceiver == null || mapQuery == null)
            {
                return;
            }

            if (totalLimit > 0 && totalSpawned >= totalLimit)
            {
                return;
            }

            if (currentTime < nextSpawnTime)
            {
                return;
            }

            bool processingInitialBurst = remainingInitialBurst > 0;
            while (remainingInitialBurst > 0 && HasCapacity())
            {
                if (!TrySpawn(mapQuery))
                {
                    break;
                }

                remainingInitialBurst--;
                if (totalLimit > 0 && totalSpawned >= totalLimit)
                {
                    break;
                }
            }

            if (!processingInitialBurst && HasCapacity())
            {
                TrySpawn(mapQuery);
            }

            nextSpawnTime = currentTime + GetNextInterval();
        }

        public bool NotifyReleased(GameObject instance)
        {
            if (instance == null)
            {
                PruneInactiveInstances();
                return false;
            }

            return activeInstances.Remove(instance);
        }

        private bool TrySpawn(IMapQuery mapQuery)
        {
            if (!area.TryGetRandomWalkablePosition(mapQuery, out Vector3 position))
            {
                return false;
            }

            SpawnRequest request = default;
            request.ChannelId = channelId;
            request.Kind = kind;
            request.Position = position;
            if (!spawnReceiver.TrySpawn(in request, out GameObject spawnedInstance))
            {
                return false;
            }

            if (spawnedInstance == null)
            {
                return false;
            }

            activeInstances.Add(spawnedInstance);
            totalSpawned++;
            return true;
        }

        private bool HasCapacity()
        {
            if (totalLimit > 0 && totalSpawned >= totalLimit)
            {
                return false;
            }

            return maximumActive <= 0 || activeInstances.Count < maximumActive;
        }

        private float GetNextInterval()
        {
            if (!useRandomInterval)
            {
                return Mathf.Max(0.01f, spawnInterval);
            }

            float minimum = Mathf.Min(randomIntervalRange.x, randomIntervalRange.y);
            float maximum = Mathf.Max(randomIntervalRange.x, randomIntervalRange.y);
            return Mathf.Max(0.01f, UnityEngine.Random.Range(minimum, maximum));
        }

        private void PruneInactiveInstances()
        {
            for (int i = activeInstances.Count - 1; i >= 0; i--)
            {
                GameObject instance = activeInstances[i];
                if (instance != null && instance.activeInHierarchy)
                {
                    continue;
                }

                activeInstances.RemoveAt(i);
            }
        }
    }

    public sealed class MapSpawnScheduler : MonoBehaviour
    {
        [SerializeField] private TilemapMap map;
        [SerializeField] private SpawnChannel[] channels = Array.Empty<SpawnChannel>();

        private void OnEnable()
        {
            ResetRuntime(Time.time);
        }

        private void Update()
        {
            if (map == null)
            {
                return;
            }

            float currentTime = Time.time;
            for (int i = 0; i < channels.Length; i++)
            {
                SpawnChannel channel = channels[i];
                if (channel == null)
                {
                    continue;
                }

                channel.Tick(map, currentTime);
            }
        }

        public void Configure(TilemapMap targetMap, SpawnChannel[] spawnChannels)
        {
            map = targetMap;
            channels = spawnChannels;
            ResetRuntime(Time.time);
        }

        public bool NotifyReleased(string channelId, GameObject instance)
        {
            for (int i = 0; i < channels.Length; i++)
            {
                SpawnChannel channel = channels[i];
                if (channel == null || channel.ChannelId != channelId)
                {
                    continue;
                }

                return channel.NotifyReleased(instance);
            }

            return false;
        }

        private void ResetRuntime(float currentTime)
        {
            for (int i = 0; i < channels.Length; i++)
            {
                SpawnChannel channel = channels[i];
                if (channel == null)
                {
                    continue;
                }

                channel.ResetRuntime(currentTime);
            }
        }
    }
}
