using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SmartPoint.Utility
{
    public static class MathUtility
    {
        /// <summary>
        /// Determine max percentage chance value can be such that all numbers sum to 100.
        /// </summary>
        public static float CalculateMaxSpawnChance(float val, float[] chances)
        {
            float sum = chances.Sum();
            return Mathf.Clamp(val, 0, 100 - (sum - val));
        }
        /// <summary>
        /// Returns true if sum is close to 100
        /// </summary>
        public static bool SumTo100(float[] chances)
        {
            if (Mathf.Approximately(chances.Sum(), 100))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Changes val so that all entities have a roughly equal spawn chance
        /// </summary>
        public static float EqualSpawnChance(int count, float[] chances, float val)
        {
            float equalParts = Mathf.Floor(100f / count * 100f) / 100f;
            bool equalBool = val == equalParts || val == equalParts + 0.01f;
            float sum = chances.Sum();
            if (100 - equalParts * count == 0 || (sum > 100 && !equalBool) || (sum == 100 && equalBool))
            {
                return equalParts;
            }
            else if (sum == 100 && equalBool)
            {
                return val;
            }
            else
            {
                return equalParts + 0.01f;
            }
        }
        /// <summary>
        /// Get a random point in circle
        /// </summary>
        public static Vector3 GetPointInCircle(Vector3 center, Vector2 circleRadius)
        {
            float dir = Random.Range(0f, Mathf.PI * 2f);
            float dist = Random.Range(circleRadius.x, circleRadius.y);
            return center + new Vector3(Mathf.Cos(dir) * dist, 0f, Mathf.Sin(dir) * dist);
        }
        /// <summary>
        /// Get a random point in rectangle
        /// </summary>
        public static Vector3 GetPointInRect(Vector3 center, Vector2 rectSize)
        {
            float randomX = Random.Range(-rectSize.x, rectSize.x);
            float randomY = Random.Range(-rectSize.y, rectSize.y);
            return center + new Vector3(randomX / 2f, 0f, randomY / 2f);
        }
        /// <summary>
        /// Get a random point in sphere
        /// </summary>
        public static Vector3 GetPointInSphere(Vector3 center, float sphereRadius)
        {
            Vector3 temp = Random.insideUnitSphere * sphereRadius;
            return center + temp;
        }
        /// <summary>
        /// Get a random point in rectangular prism
        /// </summary>
        public static Vector3 GetPointInRectPrism(Vector3 center, Vector3 rectPrismSize)
        {
            float x = Random.Range(-rectPrismSize.x, rectPrismSize.x);
            float y = Random.Range(-rectPrismSize.y, rectPrismSize.y);
            float z = Random.Range(-rectPrismSize.z, rectPrismSize.z);
            return center + new Vector3(x / 2f, y / 2f, z / 2f);
        }
        public static Vector3 ZeroY(this Vector3 vec)
        {
            return new Vector3(vec.x, 0f, vec.z);
        }
    }
}
