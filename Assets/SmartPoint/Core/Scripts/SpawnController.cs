using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SmartPoint.Utility;
using SmartPoint.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SmartPoint {

    [System.Serializable]
    public class SpawnController : MonoBehaviour
    {
        #region Fields
        //CheckpointController
        [SerializeField]
        private CheckPointController CP_Controller = null;
        //Enums
        [SerializeField]
        private SpawnMode spawnMode = SpawnMode.Manual;
        [SerializeField]
        private SpawnArea spawnArea = SpawnArea.Point;
        [SerializeField]
        private SpawnLocation spawnLocation = SpawnLocation.HighestIndex;
        [SerializeField]
        private SpawnDirection spawnDirection = SpawnDirection.Custom;
        [SerializeField]
        private ExitCondition exitCondition = ExitCondition.None;
        [SerializeField]
        private float exitTime = 10f;
        [SerializeField]
        private float exitSpawnCount = 5f;
        [SerializeField]
        private float customDirection = 0f;
        [SerializeField]
        private bool includeYPlane = false;
        //Entity stuff
        [SerializeField]
        private List<Entity> entityPool = new List<Entity>();//what spawns and what percentage they have of spawning
        [SerializeField]
        private float spawnDelay = 1f; //seconds
        [SerializeField]
        private int amountToSpawn = 1;
        [SerializeField]
        private float refillDelay = 0f;
        [SerializeField]
        private bool enableRefillMax = false;
        [SerializeField]
        private int refillMax = 1;
        [SerializeField]
        private bool enableSpawnMax = false;
        [SerializeField]
        private int constSpawnMax = 1;
        //Spawn Position
        [SerializeField]
        private bool checkForCollision = false; //Probably use overlap sphere or raycast
        [SerializeField]
        private Vector3 offsetSpawn = Vector3.zero;
        [SerializeField]
        private Vector2 circleRadius = new Vector2(0f, 1f);
        [SerializeField]
        private float sphereRadius = 0f;
        [SerializeField]
        private Vector2 rectSize = Vector2.one;
        /// <summary>
        /// [INTERNAL USE ONLY] This is only public as a workaround to a poor design feature of handles and serialized properties. Do not touch.
        /// Nevermind, fixed. Will remove on next update.
        /// </summary>
        [SerializeField]
        private Vector3 rectPrismSize = Vector3.one;
        [SerializeField]
        private bool equalSpawnChance = false;
        [SerializeField]
        private Transform targetEntity;
        //Events
        [SerializeField]
        private SpawnEvent onSpawnEvent = new SpawnEvent();
        [SerializeField]
        private ExitEvent onExitEvent = new ExitEvent();
        //Runtime
        //Used for constant
        private List<GameObject> spawnedEntities = new List<GameObject>();
        private int spawnCount = 0;
        //Used for refill
        private Dictionary<CheckPoint, GameObject> refillEntities = new Dictionary<CheckPoint, GameObject>();
        private GameObject entityParent;
        private GameObject placeholder;
        #if UNITY_EDITOR
        [SerializeField]
        private bool hideGizmos = false;
        #endif
        #endregion

        void Start()
        {
            #if UNITY_EDITOR
            GetRidOfWarnings(equalSpawnChance);
            #endif
            //Placeholder
            placeholder = new GameObject();
            placeholder.hideFlags = HideFlags.HideAndDontSave;
            //Find CheckpointController
            if (CP_Controller == null)
            {
                if (TryGetComponent(out CheckPointController cpc))
                {
                    CP_Controller = cpc;
                }
                else
                {
                    GameObject tempCPC = GameObject.Find("CheckpointController");
                    if (tempCPC != null)
                    {
                        CP_Controller = GetComponent<CheckPointController>();
                    }
                    else
                    {
                        Debug.LogError("No Checkpoint controller assigned. Failed to run spawn controller");
                        return;
                    }
                }
            }
            CleanEntityPool();
            SetAccumulatedWeights();
            //Setup parent in hierarchy
            entityParent = new GameObject("SpawnedEntities");
            SpawnModeInit();
        }

        #region Private Methods
        /// <summary>
        /// Get rid of unused variable warnings.
        /// </summary>
        private void GetRidOfWarnings(bool unused)
        {
            if (unused)
            {
                return;
            }
        }
        /// <summary>
        /// Spawn mode runtime startup.
        /// </summary>
        private void SpawnModeInit()
        {
            spawnCount = 0;
            switch (spawnMode)
            {
                case SpawnMode.SingleBurst:
                    for(int i = 0; i < amountToSpawn; i++)
                    {
                        Spawn();
                    }
                    break;
                case SpawnMode.Constant:
                    StartCoroutine(ConstantSpawn());
                    break;
                case SpawnMode.Refill:
                    StartCoroutine(RefillSpawn());
                    break;
            }
        }
        /// <summary>
        /// Get the checkpoint that the entity will spawn at.
        /// </summary>
        private CheckPoint GetSpawnCheckpoint()
        {
            //No checkpoints to teleport to
            if (CP_Controller.GetActiveCheckpoints().Length == 0)
            {
                Debug.LogWarning("There are currently no active checkpoints. Failed to Spawn.");
                return null;
            }
            switch (spawnLocation)
            {
                case SpawnLocation.HighestIndex:
                    return CP_Controller.GetLatestCheckpoint();
                case SpawnLocation.MostRecentlyActivated:
                    return CP_Controller.GetRecentlyActivatedCheckpoint();
                case SpawnLocation.NearestToEntity:
                    return CP_Controller.GetNearestCheckpoint(targetEntity.position);
                case SpawnLocation.Random:
                    return CP_Controller.GetRandomCheckpoint();
            }
            return null;
        }
        /// <summary>
        /// Returns spawn position based on SpawnArea enum
        /// </summary>
        private Vector3 SpawnPosHelper(CheckPoint cp, bool applyOffset = true)
        {
            Vector3 cpPos = cp.GetAbsolutePosition();
            Vector3 ret = Vector3.zero;
            if (applyOffset)
            {
                cpPos += offsetSpawn;
            }
            switch (spawnArea)
            {
                case SpawnArea.Point:
                    ret = cpPos;
                    break;
                case SpawnArea.Circle:
                    ret = MathUtility.GetPointInCircle(cpPos, circleRadius);
                    break;
                case SpawnArea.Rect:
                    ret = MathUtility.GetPointInRect(cpPos, rectSize);
                    break;
                case SpawnArea.Sphere:
                    ret = MathUtility.GetPointInSphere(cpPos, sphereRadius);
                    break;
                case SpawnArea.RectPrism:
                    ret = MathUtility.GetPointInRectPrism(cpPos, rectPrismSize);
                    break;
                case SpawnArea.UseCheckpointCollider:
                    ret = MathUtility.GetPointInRectPrism(cpPos, cp.GetBounds().size);
                    break;

            }
            return ret;
        }
        /// <summary>
        /// Retries the same checkpoint if check for collision is enabled.
        /// </summary>
        private Vector3? CalculateSpawnPos(CheckPoint cp, bool applyOffset = true)
        {
            int triesMax = 3;
            Vector3 ret = SpawnPosHelper(cp, applyOffset);
            int tries = spawnArea == SpawnArea.Point ? triesMax-1 : 0;
            if (checkForCollision)
            {
                while (Physics.OverlapSphere(ret, GlobalSettings.CheckpointRadius).Where(x => !x.name.Contains("Checkpoint_")).ToArray().Length > 0 && tries < triesMax)
                {
                    ret = SpawnPosHelper(cp, applyOffset);
                    tries += 1;
                }
            }
            if (tries < triesMax) { return ret; }
            else { return null; }
        }
        /// <summary>
        /// Determine angle about y-axis
        /// </summary>
        private Quaternion CalculateSpawnDirection(Vector3 entPos, float dir = 0f)
        {
            switch (spawnDirection)
            {
                case SpawnDirection.Custom:
                    return Quaternion.Euler(0f, customDirection, 0f);
                case SpawnDirection.AlignWithCheckpoint:
                    return Quaternion.Euler(new Vector3(0, dir, 0));
                case SpawnDirection.FaceEntity:
                    if (targetEntity != null)
                    {
                        if (includeYPlane)
                        {
                            return Quaternion.LookRotation((targetEntity.position - entPos).normalized);
                        }
                        return Quaternion.LookRotation((targetEntity.position - entPos).normalized.ZeroY());
                    }
                    else
                    {
                        break;
                    }
                case SpawnDirection.Random:
                    return Quaternion.Euler(new Vector3(0f, Random.Range(0, 360), 0f));
            }
            return Quaternion.identity;
        }
        /// <summary>
        /// Coroutine for when constant spawn is selected
        /// </summary>
        private IEnumerator ConstantSpawn()
        {
            if(exitCondition == ExitCondition.Time)
            {
                StartCoroutine(ExitTimer(exitTime));
            }
            while (true)
            {
                yield return new WaitForSeconds(spawnDelay);
                spawnedEntities = spawnedEntities.Where(x => x != null).ToList();
                if(!enableSpawnMax || spawnedEntities.Count < constSpawnMax)
                {
                    GameObject tempSpawn = Spawn();
                    if (tempSpawn != null)
                    {
                        spawnedEntities.Add(tempSpawn);
                        spawnCount += 1;
                    }
                    if(exitCondition == ExitCondition.SpawnCount && spawnCount >= exitSpawnCount)
                    {
                        onExitEvent.Invoke();
                        yield break;
                    }
                }
            }
        }
        /// <summary>
        /// Coroutine for when refill spawn is selected.
        /// </summary>
        private IEnumerator RefillSpawn()
        {
            refillEntities.Clear();
            if (exitCondition == ExitCondition.Time)
            {
                StartCoroutine(ExitTimer(exitTime));
            }
            while (true)
            {
                UpdateRefillList();
                //Find all null values in spawned entities. This means they've been destroyed.
                CheckPoint[] nullKeys = refillEntities.Where(x => x.Value == null).Select(x => x.Key).ToArray();
                for(int i = 0; i < nullKeys.Length; i++)
                {
                    if (!enableRefillMax || refillEntities.Count-nullKeys.Length+i < refillMax)
                    {
                        StartCoroutine(DelayedRefill(nullKeys[i]));
                    }
                    if (exitCondition == ExitCondition.SpawnCount && spawnCount >= exitSpawnCount)
                    {
                        onExitEvent.Invoke();
                        yield break;
                    }
                }
                yield return null;
            }
        }
        /// <summary>
        /// Helper to refill checkpoints
        /// </summary>
        private IEnumerator DelayedRefill(CheckPoint cp)
        {
            refillEntities[cp] = placeholder;
            yield return new WaitForSeconds(refillDelay);
            GameObject spawnEntity = Spawn(cp);
            if (spawnEntity != null)
            {
                refillEntities[cp] = spawnEntity;
                spawnCount += 1;
            }
            else
            {
                refillEntities[cp] = null;
            }
        }
        /// <summary>
        /// Remove inactive checkpoints and add newly active ones
        /// </summary>
        private void UpdateRefillList()
        {
            //remove inactive checkpoints
            foreach (CheckPoint key in refillEntities.Keys)
            {
                //Checkpoint no longer active
                if (!key.GetActive())
                {
                    refillEntities.Remove(key);
                }
            }
            //Add newly active checkpoints
            foreach (CheckPoint cp in CP_Controller.GetActiveCheckpoints())
            {
                if(!refillEntities.ContainsKey(cp)){
                    refillEntities[cp] = null;
                }
            }
        }
        /// /// <summary>
        /// Remove all null values from entity list
        /// </summary>
        private void CleanEntityPool()
        {
            entityPool = entityPool.Where(x => x.GetEntity() != null).ToList();
        }
        /// <summary>
        /// Set weights for entity selection later
        /// </summary>
        private void SetAccumulatedWeights()
        {
            float accWeight = 0;
            foreach(Entity e in entityPool)
            {
                e.accumulatedWeight = accWeight + e.GetSpawnChance();
                accWeight += e.GetSpawnChance();
            }
        }
        /// <summary>
        /// Draw Wire Sphere or solid sphere
        /// </summary>
        private void DrawSphere(Vector3 center, float radius, bool wire = true)
        {
            if (wire)
            {
                Gizmos.DrawWireSphere(center, radius);
            }
            else
            {
                Gizmos.DrawSphere(center, radius);
            }
        }
        /// <summary>
        /// Exit condition timer.
        /// </summary>
        private IEnumerator ExitTimer(float time)
        {
            yield return new WaitForSeconds(time);
            onExitEvent.Invoke();
            StopAllCoroutines();
        }
        #endregion

        #region Public Methods
        //Spawn Methods
        /// <summary>
        /// Spawn entity using SpawnController settings. Return null if unable to spawn.
        /// </summary>
        public GameObject Spawn()
        {
            return Spawn(null);
        }
        /// <summary>
        /// Spawn entity at specific checkpoint. Return null if unable to spawn.
        /// </summary>
        public GameObject Spawn(CheckPoint spawnCheckpoint, bool applyOffset = true)
        {
            //Called with no checkpoint in mind
            bool checkInitNull = false;
            if (spawnCheckpoint == null)
            {
                spawnCheckpoint = GetSpawnCheckpoint();
                checkInitNull = true;
            }

            //Get an entity
            GameObject spawnEntity = GetEntityFromPool();
            if (spawnEntity == null)
            {
                return null;
            }
            //Get the spawn position
            Vector3? spawnPos = CalculateSpawnPos(spawnCheckpoint, applyOffset);
            //If they didn't pass a checkpoint in and we're checking multiple points because of collision.
            if (checkInitNull && checkForCollision)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (spawnPos.HasValue)
                    {
                        break;
                    }
                    spawnCheckpoint = CP_Controller.GetRandomCheckpoint();
                    spawnPos = CalculateSpawnPos(spawnCheckpoint, applyOffset);
                }
            }
            //If after 5 tries there's still no value
            if (!spawnPos.HasValue)
            {
                return null;
            }
            Quaternion spawnDirection = CalculateSpawnDirection(spawnPos.Value, spawnCheckpoint.GetDirection());
            GameObject newEntity = Instantiate(spawnEntity, spawnPos.Value, spawnDirection, entityParent.transform);
            onSpawnEvent.Invoke(newEntity, spawnCheckpoint.GetIndex());
            return newEntity;
        }
        /// <summary>
        /// Spawn Entity at position. Return null if unable to spawn.
        /// </summary>
        public GameObject Spawn(Vector3 spawnPosition, bool applyOffset = false)
        {
            GameObject spawnEntity = GetEntityFromPool();
            if (spawnEntity == null)
            {
                return null;
            }

            if (applyOffset)
            {
                spawnPosition += offsetSpawn;
            }
            Quaternion spawnDirection = CalculateSpawnDirection(spawnPosition);
            GameObject newEntity = Instantiate(spawnEntity, spawnPosition, spawnDirection, entityParent.transform);
            onSpawnEvent.Invoke(newEntity, -1);
            return newEntity;
        }
        /// <summary>
        /// Return an entity based on spawn chance. Spawn chances to not have to sum to 100.
        /// </summary>
        //Entity Methods
        public GameObject GetEntityFromPool()
        {
            float ratioTotal = GetSpawnChances().Sum();
            float r = Random.Range(0, ratioTotal);

            foreach (Entity e in entityPool)
            {
                if (r <= e.accumulatedWeight)
                {
                    return e.GetEntity();
                }
            }
            //Only here if there are no entities
            return null;
        }
        /// <summary>
        /// Return array of all the spawn chances from Entity Pool
        /// </summary>
        public float[] GetSpawnChances()
        {
            return entityPool.Select(x => x.GetSpawnChance()).ToArray();
        }
        /// <summary>
        /// Resets the spawner, restarting all coroutines and forgetting all previously spawned entities.
        /// <br></br>
        /// All editor settings remain the same.
        /// </summary>
        public void ResetSpawner()
        {
            StopAllCoroutines();
            SetAccumulatedWeights();
            SpawnModeInit();
        }
        /// <summary>
        /// Destroy all spawned entities. Specifically those parented to 'SpawnedEntities' in hierarchy.
        /// </summary>
        public void DestroySpawnedEntities()
        {
            foreach(Transform trans in entityParent.transform)
            {
                Destroy(trans.gameObject);
            }
            spawnedEntities.Clear();
            refillEntities.Clear();
        }
        /// <summary>
        /// Adds an entity to the pool. May affect other spawn chances.
        /// </summary>
        public void AddEntity(GameObject entity, float spawnChance)
        {
            Entity newEnt = new Entity(entity, spawnChance);
            entityPool.Add(newEnt);
            if (equalSpawnChance)
            {
                 newEnt.SetSpawnChance(MathUtility.EqualSpawnChance(GetSpawnChances().Length, GetSpawnChances(), spawnChance));
            }
            else
            {
                //Clamp spawn chance to sum to 100
                newEnt.SetSpawnChance(MathUtility.CalculateMaxSpawnChance(spawnChance, GetSpawnChances()));
            }
            SetAccumulatedWeights();
        }
        /// <summary>
        /// Remove an entity from the pool.
        /// </summary>
        public void RemoveEntity(GameObject entity)
        {
            entityPool.RemoveAll(x => x.GetEntity() == entity);
            SetAccumulatedWeights();
        }
        //Getters
        /// <summary>
        /// Return array from entities from the Entity Pool
        /// </summary>
        public GameObject[] GetEntities(bool asArray)
        {
            return entityPool.Select(x => x.GetEntity()).ToArray();
        }
        /// <summary>
        /// Return List of entities from Entity Pool
        /// </summary>
        public List<Entity> GetEntities()
        {
            return entityPool;
        }
        /// <summary>
        /// Returns the target entity set in the inspector. Used only for specific modes.
        /// </summary>
        public GameObject GetTargetEntity()
        {
            return targetEntity.gameObject;
        }
        /// <summary>
        /// Return the parent object in the hierarchy.
        /// </summary>
        public Transform GetSpawnerParent()
        {
            return entityParent.transform;
        }
        /// <summary>
        /// Get the delay between spawns (Constant Mode).
        /// </summary>
        public float GetSpawnDelay()
        {
            return spawnDelay;
        }
        /// <summary>
        /// Get the delay before refill (Refill Mode).
        /// </summary>
        public float GetRefillDelay()
        {
            return refillDelay;
        }
        /// <summary>
        /// Get the max number of refill entities that can exist at once (Refill Mode).
        /// </summary>
        public int GetRefillMax()
        {
            return refillMax;
        }
        /// <summary>
        /// Get the max number of spawned entities that can exist at once (Constant Mode).
        /// </summary>
        public int GetSpawnMax()
        {
            return constSpawnMax;
        }
        public Vector3 GetRectPrismSize()
        {
            return rectPrismSize;
        }
        //Setters
        /// <summary>
        /// Sets the target entity (used only for specific modes).
        /// </summary>
        public void SetTargetEntity(Transform transform)
        {
            targetEntity = transform;
        }
        /// <summary>
        /// For Constant spawn mode. Set the delay before the next entity will spawn.
        /// </summary>
        public void SetSpawnDelay(float delay)
        {
            spawnDelay = delay;
        }
        /// <summary>
        /// For Refill mode. Set the delay before a checkpoint re-spawns an entity.
        /// </summary>
        public void SetRefillDelay(float delay)
        {
            refillDelay = delay;
        }
        /// <summary>
        /// For Refill mode. Set the max number of entities that can exist at one time in the scene.
        /// </summary>
        public void SetRefillMax(int max)
        {
            refillMax = max;
        }
        /// <summary>
        /// For Constant spawn mode. Set the max number of entities that can exist at one time in the scene.
        /// </summary>
        public void SetSpawnMax(int max)
        {
            constSpawnMax = max;
        }
        public void SetRectPrismSize(Vector3 size)
        {
            if(spawnArea == SpawnArea.RectPrism)
            {
                rectPrismSize = size;
            }
        }
        #endregion

        #region Gizmo
        #if UNITY_EDITOR
        //Draw gizmos for checkpoints
        private void OnDrawGizmosSelected()
        {
            if (!hideGizmos && CP_Controller != null)
            {
                GizmoDrawing();
            }
        }
        private void GizmoDrawing()
        {
            if (spawnArea == SpawnArea.Sphere)
            {
                List<CheckPoint> points = CP_Controller.checkPoints;
                for (int i = 0; i < points.Count; i++)
                {
                    //Set Gizmo Color
                    if (points[i].GetActive())
                    {
                        Gizmos.color = GlobalSettings.SphereColor;
                    }
                    else
                    {
                        Gizmos.color = GlobalSettings.GrayedOutColor;
                    }
                    DrawSphere(points[i].GetAbsolutePosition() + offsetSpawn, sphereRadius);
                }
            }
            else if (spawnArea == SpawnArea.Point)
            {
                List<CheckPoint> points = CP_Controller.checkPoints;
                for (int i = 0; i < points.Count; i++)
                {
                    //Set Gizmo Color
                    if (points[i].GetActive())
                    {
                        Gizmos.color = GlobalSettings.SphereColor;
                    }
                    else
                    {
                        Gizmos.color = GlobalSettings.GrayedOutColor;
                    }
                    DrawSphere(points[i].GetAbsolutePosition() + offsetSpawn, GlobalSettings.CheckpointRadius, false);
                }
            }
            else if(spawnArea == SpawnArea.UseCheckpointCollider)
            {
                List<CheckPoint> points = CP_Controller.checkPoints;
                for (int i = 0; i < points.Count; i++)
                {
                    //Set Gizmo Color
                    if (points[i].GetActive())
                    {
                        Gizmos.color = GlobalSettings.SphereColor;
                    }
                    else
                    {
                        Gizmos.color = GlobalSettings.GrayedOutFaceColor;
                    }
                    Gizmos.DrawWireCube(points[i].GetAbsolutePosition() + offsetSpawn, points[i].GetBoundsSize());
                }
            }
        }
        #endif
        #endregion
    }
}
