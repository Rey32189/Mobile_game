using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartPoint;

namespace SmartPointExample
{
    public class CP_EnemyEventExample : MonoBehaviour
    {
        public CheckPointController CP_Controller;
        private void Start()
        {
            if (CP_Controller == null)
            {
                CP_Controller = (CheckPointController)FindObjectOfType(typeof(CheckPointController));
            }
        }
        //This method triggers when colliding with an active checkpoint. This increases enemy speed temporarily.
        //See event in checkpoint controller to see callback
        public void IncreaseSpeedTemporarily(int index, GameObject entity)
        {
            StartCoroutine(entity.GetComponent<CP_EnemyExample>().IncreaseSpeed());
        }

        public void AddToList(GameObject entity, int index)
        {
            CP_Controller.AddNewEntity(entity);
        }
    }
}
