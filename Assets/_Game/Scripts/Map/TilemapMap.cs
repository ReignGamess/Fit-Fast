using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameRPG.Systems
{
    public sealed class TilemapMap : MonoBehaviour, IMapQuery
    {
        [SerializeField] private Grid grid;
        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap obstaclesTilemap;

        public BoundsInt CellBounds
        {
            get
            {
                if (groundTilemap == null)
                {
                    return default;
                }

                return groundTilemap.cellBounds;
            }
        }

        public Bounds WorldBounds
        {
            get
            {
                if (grid == null || groundTilemap == null)
                {
                    return default;
                }

                BoundsInt cellBounds = groundTilemap.cellBounds;
                Vector3 minimum = grid.CellToWorld(cellBounds.min);
                Vector3 maximum = grid.CellToWorld(cellBounds.max);
                Vector3 size = maximum - minimum;
                size.x = Mathf.Abs(size.x);
                size.y = Mathf.Abs(size.y);
                size.z = Mathf.Abs(size.z);
                return new Bounds((minimum + maximum) * 0.5f, size);
            }
        }

        public void Configure(Grid targetGrid, Tilemap ground, Tilemap obstacles)
        {
            grid = targetGrid;
            groundTilemap = ground;
            obstaclesTilemap = obstacles;
        }

        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            if (grid == null)
            {
                return default;
            }

            return grid.WorldToCell(worldPosition);
        }

        public Vector3 CellToWorldCenter(Vector3Int cellPosition)
        {
            if (grid == null)
            {
                return default;
            }

            return grid.GetCellCenterWorld(cellPosition);
        }

        public bool IsWalkable(Vector3Int cellPosition)
        {
            if (groundTilemap == null || obstaclesTilemap == null)
            {
                return false;
            }

            if (!groundTilemap.HasTile(cellPosition))
            {
                return false;
            }

            return !obstaclesTilemap.HasTile(cellPosition);
        }

        public bool IsWalkable(Vector3 worldPosition)
        {
            return IsWalkable(WorldToCell(worldPosition));
        }
    }
}
