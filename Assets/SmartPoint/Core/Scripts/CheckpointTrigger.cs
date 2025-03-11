using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SmartPoint
{
    public class CheckpointTrigger : MonoBehaviour
    {
        public int cpIndex;
        private CheckPointController cpc;
        // Start is called before the first frame update
        void Awake()
        {
            cpc = transform.parent.GetComponent<CheckPointController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (cpc.GetEntities().Contains(other.gameObject))
            {
                cpc.CollisionOccurred(cpIndex, other.gameObject);
            }
        }
    }
}
