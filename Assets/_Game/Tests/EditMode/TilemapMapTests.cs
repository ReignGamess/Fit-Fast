using GameRPG.Systems;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameRPG.Tests.EditMode
{
    public sealed class TilemapMapTests
    {
        private GameObject root;
        private Tile groundTile;
        private Tile obstacleTile;
        private Tilemap ground;
        private Tilemap obstacles;
        private TilemapMap map;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Grid", typeof(Grid), typeof(TilemapMap));
            GameObject groundObject = new("Ground", typeof(Tilemap), typeof(TilemapRenderer));
            GameObject obstaclesObject = new("Obstacles", typeof(Tilemap), typeof(TilemapRenderer));
            groundObject.transform.SetParent(root.transform);
            obstaclesObject.transform.SetParent(root.transform);
            ground = groundObject.GetComponent<Tilemap>();
            obstacles = obstaclesObject.GetComponent<Tilemap>();
            map = root.GetComponent<TilemapMap>();
            map.Configure(root.GetComponent<Grid>(), ground, obstacles);
            groundTile = ScriptableObject.CreateInstance<Tile>();
            obstacleTile = ScriptableObject.CreateInstance<Tile>();
            ground.SetTile(Vector3Int.zero, groundTile);
            ground.SetTile(Vector3Int.right, groundTile);
            obstacles.SetTile(Vector3Int.right, obstacleTile);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(groundTile);
            Object.DestroyImmediate(obstacleTile);
        }

        [Test]
        public void WalkabilityRequiresGroundWithoutObstacle()
        {
            Assert.That(map.IsWalkable(Vector3Int.zero), Is.True);
            Assert.That(map.IsWalkable(Vector3Int.right), Is.False);
            Assert.That(map.IsWalkable(Vector3Int.up), Is.False);
            Assert.That(map.IsWalkable(map.CellToWorldCenter(Vector3Int.zero)), Is.True);
        }

        [Test]
        public void CoordinateConversionAndBoundsMatchGroundTiles()
        {
            Vector3 center = map.CellToWorldCenter(Vector3Int.zero);
            Assert.That(map.WorldToCell(center), Is.EqualTo(Vector3Int.zero));
            Assert.That(map.CellBounds.Contains(Vector3Int.zero), Is.True);
            Assert.That(map.WorldBounds.size.x, Is.EqualTo(2f).Within(0.001f));
            Assert.That(map.WorldBounds.size.y, Is.EqualTo(1f).Within(0.001f));
        }
    }
}
