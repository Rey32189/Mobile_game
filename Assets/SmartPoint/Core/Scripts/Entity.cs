using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartPoint.Utility;

namespace SmartPoint {

    [System.Serializable]
    public class Entity
    {
        #region Properties
        /// <summary>
        /// [INTERNAL USE ONLY]
        /// </summary>
        [HideInInspector]
        public float accumulatedWeight;
        [SerializeField]
        private GameObject entity;
        [SerializeField]
        private float spawnChance = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an entity with 0 spawn chance and sets gameobject to e
        /// </summary>
        public Entity(GameObject e)
        {
            entity = e;
        }
        /// <summary>
        /// Creates an entity with spawn chance and sets gameobject to e
        /// </summary>
        public Entity(GameObject e, float sc)
        {
            entity = e;
            spawnChance = sc;
        }
        #endregion

        #region Getters
        /// <summary>
        /// Returns spawn chance
        /// </summary>
        public float GetSpawnChance()
        {
            return spawnChance;
        }
        /// <summary>
        /// Returns assigned GameObject
        /// </summary>
        public GameObject GetEntity()
        {
            return entity;
        }
        #endregion

        #region Setters
        /// <summary>
        /// Sets the spawnchance, ignoring max of 100. This will not break the program.
        /// </summary>
        public void SetSpawnChance(float sc)
        {
            spawnChance = sc;
        }
        /// <summary>
        /// Sets the GameObject
        /// </summary>
        public void SetEntity(GameObject go)
        {
            entity = go;
        }
        #endregion
    }
}
