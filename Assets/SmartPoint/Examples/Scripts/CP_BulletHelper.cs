using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartPoint;

namespace SmartPointExample {
    public class CP_BulletHelper : MonoBehaviour
    {
        private CheckPointController CP_Controller;
        private SpawnController SP_Controller;
        void Start()
        {
            if (CP_Controller == null)
            {
                CP_Controller = (CheckPointController)FindObjectOfType(typeof(CheckPointController));
            }
            if (SP_Controller == null)
            {
                SP_Controller = (SpawnController)FindObjectOfType(typeof(SpawnController));
            }
        }

        Color[] colors = new Color[] { Color.red, Color.blue, Color.yellow, Color.white };

        public void SetBulletDirection(GameObject go, int index)
        {
            go.GetComponent<CP_BulletExample>().SetParameters(3f, 3f);
        }
        public void SetBulletTrailColor(GameObject go, int index)
        {
            Material mat = go.GetComponent<TrailRenderer>().material;
            mat.SetColor("_EmissionColor", colors[index]);
        }
        public void SpawnBullet(int index)
        {
            SP_Controller.Spawn(CP_Controller.GetCheckpoint(index));
        }
    }
}
