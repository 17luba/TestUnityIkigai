using UnityEngine;

namespace Ikigai.DontTouchTheSpikes.Utils
{
    public static class VectorExtensions
    {
        public static Vector2 GetScreenPosFromWorldPos(this Vector3 worldPosition, RectTransform canvasRect, Camera camera)
        {
            Vector2 viewportPos = camera.WorldToViewportPoint(worldPosition);
            Vector2 sizeDelta = canvasRect.sizeDelta;
            return new Vector2(viewportPos.x * sizeDelta.x - sizeDelta.x * 0.5f,
                viewportPos.y * sizeDelta.y - sizeDelta.y * 0.5f);
        }
    }
}