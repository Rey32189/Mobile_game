using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SmartPoint {
    /// <summary>
    /// CheckPoint Class contains properties for active state, position, and size (only used for colliderMode).
    /// </summary>
    [System.Serializable]
    public class CheckPoint
    {
        #region Properties
        /// <summary>
        /// [INTERNAL USE ONLY] Access to the checkpoint controller that created it.
        /// </summary>
        [HideInInspector]
        public CheckPointController cpc;
        [SerializeField]
        private bool isActive;
        //Important to note this is the relative position (from checkpoint controller)
        [SerializeField]
        private Vector3 position;
        [SerializeField]
        private Bounds bounds;
        [SerializeField]
        private float direction;
        #endregion

        #region Constructors
        public CheckPoint(Vector3 pos, bool activeState)
        {
            SetActive(activeState);
            SetPosition(pos);
            SetBounds(new Bounds(GetAbsolutePosition(), Vector3.one));
        }
        public CheckPoint(Vector3 pos, bool activeState, Bounds newBounds)
        {
            SetActive(activeState);
            SetPosition(pos);
            SetBounds(newBounds);
        }
        #endregion

        #region Getters
        public bool GetActive()
        {
            return isActive;
        }
        public Bounds GetBounds()
        {
            return bounds;
        }
        public Vector3 GetBoundsSize()
        {
            return bounds.size;
        }
        public Vector3 GetBoundsCenter()
        {
            return bounds.center;
        }
        public float GetDirection()
        {
            return direction;
        }
        public Vector3 GetPosition()
        {
            return position;
        }
        public Vector3 GetAbsolutePosition()
        {
            //Bug with 2019.4
            if (cpc == null) return position;
            return position + cpc.transform.position;
        }
        public int GetIndex()
        {
            return cpc.checkPoints.IndexOf(this);
        }
        #endregion

        #region Setters
        /// <summary>
        /// Setting a checkpoints state directly here circumvents any settings and conditions for activation (such as sequential activation).
        /// <br></br>
        /// To follow settings, use SetCheckpointState in CheckPointController
        /// </summary>
        public void SetActive(bool state)
        {
            isActive = state;
        }
        /// <summary>
        /// Set relative position
        /// </summary>
        public void SetPosition(Vector3 pos)
        {
            position = pos;
        }
        /// <summary>
        /// Set absolute position
        /// </summary>
        public void SetAbsolutePosition(Vector3 pos)
        {
            position = pos - cpc.transform.position;
        }
        /// <summary>
        /// Bounds for trigger box. Used for ActivateOnCollision.
        /// </summary>
        public void SetBounds(Bounds b)
        {
            bounds = b;
        }
        public void SetBoundsSize(Vector3 bs)
        {
            bounds.size = bs;
        }
        public void SetBoundsCenter(Vector3 bc)
        {
            bounds.center = bc;
        }
        public void SetDirection(float dir)
        {
            //No negatives
            if (dir < 0)
            {
                dir += 360;
            }
            //Limit max
            else if (dir > 360)
            {
                dir = dir % 360;
            }
            direction = dir;
        }
        #endregion
    }
}
