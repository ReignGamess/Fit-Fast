using UnityEngine;

namespace GameRPG.Systems
{
    public enum SpawnKind
    {
        Enemy,
        Food
    }

    public struct SpawnRequest
    {
        public string ChannelId { get; internal set; }
        public SpawnKind Kind { get; internal set; }
        public Vector3 Position { get; internal set; }
    }

    public interface ISpawnReceiver
    {
        bool TrySpawn(in SpawnRequest request, out GameObject spawnedInstance);
    }
}
