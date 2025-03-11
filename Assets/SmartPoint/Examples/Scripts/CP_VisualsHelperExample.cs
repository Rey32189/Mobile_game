using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartPoint;

namespace SmartPointExample
{
    public class CP_VisualsHelperExample : MonoBehaviour
    {
        public Material inactiveMat;
        public Material activeMat;

        public void ChangeMarkerColorActive(int index)
        {
            FindMarker(index).material = activeMat;
        }
        public void ChangeMarkerColorInactive(int index)
        {
            FindMarker(index).material = inactiveMat;
        }
        private MeshRenderer FindMarker(int index)
        {
            return transform.GetChild(index).GetComponent<MeshRenderer>();
        }
        public void DelayedDeactivate(int index)
        {
            StartCoroutine(DeactivateAfter2Seconds(index));
        }
        private IEnumerator DeactivateAfter2Seconds(int index)
        {
            yield return new WaitForSeconds(2);
            ((CheckPointController)FindObjectOfType(typeof(CheckPointController))).SetCheckpointState(index, false);
        }
    }
}
