using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartPoint
{
    public class ProximityChecker : MonoBehaviour
    {
        public int cpIndex;
        public float distance = 0f;
        private CheckPointController cpp;
        private GameObject[] entities;
        private void Start()
        {
            cpp = transform.parent.GetComponent<CheckPointController>();
            entities = cpp.GetEntities();
        }

        void Update()
        {
            if (!cpp.GetCheckpointState(cpIndex))
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (WithinDistance(entities[i].transform.position, distance))
                    {
                        cpp.SetCheckpointState(cpIndex, true);
                    }
                }
            }
        }
        private bool WithinDistance(Vector3 entityPos, float distance)
        {
            if (Vector3.Distance(entityPos, transform.position) <= distance)
            {
                return true;
            }
            return false;
        }
    }
}