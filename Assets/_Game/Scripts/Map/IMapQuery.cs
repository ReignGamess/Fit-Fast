using UnityEngine;

namespace GameRPG.Systems
{
    public interface IMapQuery
    {
        BoundsInt CellBounds { get; }
        Bounds WorldBounds { get; }

        Vector3Int WorldToCell(Vector3 worldPosition);
        Vector3 CellToWorldCenter(Vector3Int cellPosition);
        bool IsWalkable(Vector3Int cellPosition);
        bool IsWalkable(Vector3 worldPosition);
    }
}
