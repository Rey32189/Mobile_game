using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SmartPoint
{
    public class OutOfBoundsHelper : MonoBehaviour
    {
        [SerializeField]
        private CheckPointController CP_Controller;
        [SerializeField]
        private TeleportMode teleportMode = TeleportMode.HighestIndex;
        [Tooltip("If any of your entities use a CharacterController or Rigidbody, reset velocity to 0 on teleport.")]
        [SerializeField]
        private bool resetVelocity = true;
        // Start is called before the first frame update
        private void Start()
        {
            if (CP_Controller == null)
            {
                CP_Controller = (CheckPointController)FindObjectOfType(typeof(CheckPointController));
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            //Check to make sure entity is in the CPC entity list
            if (CP_Controller.GetEntities().Contains(other.gameObject))
            {
                if (other.TryGetComponent(out CharacterController comp))
                {
                    //Reset velocity
                    if (resetVelocity)
                    {
                        comp.SimpleMove(Vector3.zero);
                    }
                    //Teleport after deactiving character controller
                    comp.enabled = false;
                    TeleportEntity(other.gameObject);
                    comp.enabled = true;
                }
                else
                {
                    if (resetVelocity && other.TryGetComponent<Rigidbody>(out Rigidbody m_rigid))
                    {
                        m_rigid.velocity = m_rigid.angularVelocity = Vector3.zero;
                    }
                    //Teleport
                    TeleportEntity(other.gameObject);
                }
            }
        }
        //Check teleport mode. This is just a way of determining which checkpoint to teleport to. See docs for more info
        private void TeleportEntity(GameObject go)
        {
            if (teleportMode == TeleportMode.Nearest)
            {
                CP_Controller.TeleportToNearest(go);
            }
            else if (teleportMode == TeleportMode.HighestIndex)
            {
                CP_Controller.TeleportToLatest(go);
            }
            else if (teleportMode == TeleportMode.MostRecentlyActivated)
            {
                CP_Controller.TeleportToRecentlyActivated(go);
            }
            else if (teleportMode == TeleportMode.Random)
            {
                CP_Controller.TeleportToRandom(go);
            }
        }
    }
}
