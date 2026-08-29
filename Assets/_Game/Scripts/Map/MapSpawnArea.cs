using UnityEngine;

namespace GameRPG.Systems
{
    public sealed class MapSpawnArea : MonoBehaviour
    {
        [SerializeField] private TilemapMap map;
        [SerializeField] private Vector2Int minimumCell = new(-15, -9);
        [SerializeField] private Vector2Int maximumCellExclusive = new(15, 9);
        [SerializeField] private int randomAttempts = 32;

        public BoundsInt CellBounds
        {
            get
            {
                Vector3Int position = new(minimumCell.x, minimumCell.y, 0);
                Vector3Int size = new(
                    Mathf.Max(0, maximumCellExclusive.x - minimumCell.x),
                    Mathf.Max(0, maximumCellExclusive.y - minimumCell.y),
                    1);
                return new BoundsInt(position, size);
            }
        }

        public void Configure(TilemapMap targetMap, Vector2Int minimum, Vector2Int maximumExclusive, int attempts = 32)
        {
            map = targetMap;
            minimumCell = minimum;
            maximumCellExclusive = maximumExclusive;
            randomAttempts = Mathf.Max(1, attempts);
        }

        public bool TryGetRandomWalkablePosition(IMapQuery mapQuery, out Vector3 worldPosition)
        {
            worldPosition = default;
            if (mapQuery == null)
            {
                return false;
            }

            if (maximumCellExclusive.x <= minimumCell.x || maximumCellExclusive.y <= minimumCell.y)
            {
                return false;
            }

            int attemptCount = Mathf.Max(1, randomAttempts);
            for (int i = 0; i < attemptCount; i++)
            {
                Vector3Int cell = default;
                cell.x = Random.Range(minimumCell.x, maximumCellExclusive.x);
                cell.y = Random.Range(minimumCell.y, maximumCellExclusive.y);
                if (!mapQuery.IsWalkable(cell))
                {
                    continue;
                }

                worldPosition = mapQuery.CellToWorldCenter(cell);
                return true;
            }

            for (int y = minimumCell.y; y < maximumCellExclusive.y; y++)
            {
                for (int x = minimumCell.x; x < maximumCellExclusive.x; x++)
                {
                    Vector3Int cell = default;
                    cell.x = x;
                    cell.y = y;
                    if (!mapQuery.IsWalkable(cell))
                    {
                        continue;
                    }

                    worldPosition = mapQuery.CellToWorldCenter(cell);
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            if (map == null)
            {
                return;
            }

            BoundsInt cells = CellBounds;
            Vector3 minimum = map.CellToWorldCenter(cells.min) - Vector3.one * 0.5f;
            Vector3 maximum = map.CellToWorldCenter(cells.max - Vector3Int.one) + Vector3.one * 0.5f;
            Vector3 size = maximum - minimum;
            size.z = 0f;
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.8f);
            Gizmos.DrawWireCube((minimum + maximum) * 0.5f, size);
        }
    }
}
