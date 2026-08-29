using GameRPG.Systems;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameRPG.Tests.EditMode
{
    public sealed class MapCameraBoundsTests
    {
        private GameObject gridObject;
        private GameObject cameraObject;
        private Tile groundTile;
        private TilemapMap map;
        private Camera targetCamera;
        private MapCameraBounds cameraBounds;

        [SetUp]
        public void SetUp()
        {
            gridObject = new GameObject("Grid", typeof(Grid), typeof(TilemapMap));
            GameObject groundObject = new("Ground", typeof(Tilemap), typeof(TilemapRenderer));
            GameObject obstaclesObject = new("Obstacles", typeof(Tilemap), typeof(TilemapRenderer));
            groundObject.transform.SetParent(gridObject.transform);
            obstaclesObject.transform.SetParent(gridObject.transform);
            Tilemap ground = groundObject.GetComponent<Tilemap>();
            groundTile = ScriptableObject.CreateInstance<Tile>();
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    ground.SetTile(new Vector3Int(x, y), groundTile);
                }
            }

            map = gridObject.GetComponent<TilemapMap>();
            map.Configure(gridObject.GetComponent<Grid>(), ground, obstaclesObject.GetComponent<Tilemap>());
            cameraObject = new GameObject("Camera", typeof(Camera), typeof(MapCameraBounds));
            targetCamera = cameraObject.GetComponent<Camera>();
            targetCamera.orthographic = true;
            cameraBounds = cameraObject.GetComponent<MapCameraBounds>();
            cameraBounds.Configure(targetCamera, map);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(cameraObject);
            Object.DestroyImmediate(gridObject);
            Object.DestroyImmediate(groundTile);
        }

        [TestCase(1.777778f)]
        [TestCase(2.222222f)]
        public void CameraClampsInsideMapAtMobileAspectRatios(float aspect)
        {
            targetCamera.orthographicSize = 0.5f;
            targetCamera.aspect = aspect;
            cameraObject.transform.position = new Vector3(-10f, 10f, -10f);
            cameraBounds.ConstrainNow();
            float halfWidth = targetCamera.orthographicSize * aspect;
            Assert.That(cameraObject.transform.position.x, Is.EqualTo(halfWidth).Within(0.001f));
            Assert.That(cameraObject.transform.position.y, Is.EqualTo(3.5f).Within(0.001f));
            Assert.That(cameraObject.transform.position.z, Is.EqualTo(-10f));
        }

        [Test]
        public void CameraCentersWhenViewportIsLargerThanMap()
        {
            targetCamera.orthographicSize = 5f;
            targetCamera.aspect = 2f;
            cameraObject.transform.position = new Vector3(20f, -20f, -10f);
            cameraBounds.ConstrainNow();
            Assert.That(cameraObject.transform.position.x, Is.EqualTo(2f).Within(0.001f));
            Assert.That(cameraObject.transform.position.y, Is.EqualTo(2f).Within(0.001f));
        }
    }
}
