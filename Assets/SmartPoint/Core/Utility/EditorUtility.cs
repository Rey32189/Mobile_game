using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SmartPoint.Utility
{
    public static class DisplayUtility
    {
    #if UNITY_EDITOR
        /// <summary>
        /// Function which returns true if mouse is hovering on a sphere gizmo
        /// </summary>
        /// <returns></returns>
        public static bool SphereMouseOverlap(Vector3 spherePos, float radius, Camera cam, float fov, float screenHeight)
        {
            //Calculate projected radius
            //float pr = CalculateProjectedRadius(spherePos, radius, cam.transform.position, fov, screenHeight);
            //Determine if a gizmo is within mouse range
            //Vector3 pos = cam.WorldToScreenPoint(spherePos);
            float dst = HandleUtility.DistanceToCircle(spherePos, radius);
            if (dst == 0)
                return true;
            return false;
        }
        /// <summary>
        /// Returns true if a vector3 point is visible from the editor sceneview.
        /// </summary>
        /// <returns></returns>
        public static bool PointOnScreen(Camera cam, Vector3 pos)
        {
            Vector2 point = cam.WorldToScreenPoint(pos);
            return point.x > 0 && point.x < cam.pixelWidth && point.y > 0 && point.y < cam.pixelHeight;
        }
        /// <summary>
        /// Determine where the label should be positioned based on the text size
        /// </summary>
        /// <returns></returns>
        public static Vector3 CalculateLabelPosition(Camera cam, Vector3 spherePos, GUIStyle gs, string text)
        {
            Vector2 size = gs.CalcSize(new GUIContent(text));
            Vector3 point = cam.WorldToScreenPoint(spherePos);
            point.y += size.y / 3f;
            point.x -= size.x / 5f;
            return cam.ScreenToWorldPoint(point);
        }
        /// <summary>
        /// Determines whether or not numbers should be rendered for a checkpoint. This is based on camera distance from screen.
        /// </summary>
        public static bool IsSceneViewCameraInRange(Camera cam, Vector3 position, float distance)
        {
            Vector3 cameraPos = cam.WorldToScreenPoint(position);
            return (cameraPos.x >= 0) &&
            (cameraPos.x <= cam.pixelWidth) &&
            (cameraPos.y >= 0) &&
            (cameraPos.y <= cam.pixelHeight) &&
            (cameraPos.z > 0) &&
            (cameraPos.z < distance);
        }
    #endif
    }
}