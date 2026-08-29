using UnityEngine;

namespace GameRPG.Systems
{
    [DefaultExecutionOrder(1000)]
    public sealed class MapCameraBounds : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private TilemapMap map;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = GetComponent<Camera>();
            }
        }

        private void LateUpdate()
        {
            ConstrainNow();
        }

        public void Configure(Camera cameraToConstrain, TilemapMap targetMap)
        {
            targetCamera = cameraToConstrain;
            map = targetMap;
        }

        public void ConstrainNow()
        {
            if (targetCamera == null || map == null || !targetCamera.orthographic)
            {
                return;
            }

            Bounds bounds = map.WorldBounds;
            if (bounds.size.x <= 0f || bounds.size.y <= 0f)
            {
                return;
            }

            float halfHeight = targetCamera.orthographicSize;
            float halfWidth = halfHeight * targetCamera.aspect;
            Vector3 position = transform.position;
            position.x = ClampAxis(position.x, bounds.min.x, bounds.max.x, halfWidth);
            position.y = ClampAxis(position.y, bounds.min.y, bounds.max.y, halfHeight);
            transform.position = position;
        }

        private static float ClampAxis(float value, float minimum, float maximum, float halfViewport)
        {
            float availableSize = maximum - minimum;
            if (availableSize <= halfViewport * 2f)
            {
                return (minimum + maximum) * 0.5f;
            }

            return Mathf.Clamp(value, minimum + halfViewport, maximum - halfViewport);
        }
    }
}
