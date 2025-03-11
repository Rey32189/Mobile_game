using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartPoint;
using System.Linq;

namespace SmartPointExample {
    public class CP_AmmoConsume : MonoBehaviour
    {
        [SerializeField]
        private CheckPointController CP_Controller;

        // Start is called before the first frame update
        private void Start()
        {
            if (CP_Controller == null)
            {
                CP_Controller = GameObject.Find("CheckpointController").GetComponent<CheckPointController>();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            //Check to make sure entity is in the CPC entity list
            if (CP_Controller.GetEntities().Contains(other.gameObject))
            {
                Destroy(gameObject);
            }
        }
    }
}
