using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SmartPoint.Events;

namespace SmartPoint
{
    //TODO: Colliders at runtime. Moving checkpoints at runtime. MRA when checkpoint disabled (queue).
    [System.Serializable]
    public class CheckPointController : MonoBehaviour
    {
        #region Fields
        //Editor Only fields
        #if UNITY_EDITOR
        [SerializeField]
        private bool HideGizmos = false;
        [SerializeField]
        private bool ShowAbsolutePosition = false;
        [SerializeField]
        private float guiScale = 1.6f;
        //Custom variable used when detecting if list size increased
        [SerializeField]
        [HideInInspector]
        private int checkPointCount = 0;
        #endif

        ///<summary>
        ///[INTERNAL USE ONLY]. List used to store all the checkpoints.
        ///</summary>
        [SerializeField]
        public List<CheckPoint> checkPoints = new List<CheckPoint>();

        ///<summary>
        ///[INTERNAL USE ONLY]. List used to store all the selected checkpoints in sceneview.
        ///</summary>
        public List<int> selectedIndices = new List<int>();

        //Modes
        [SerializeField]
        private EntityMode entityMode = EntityMode.SingleEntity;
        [SerializeField]
        private ActivationMode activationMode = ActivationMode.ManualActivation;
        [SerializeField]
        private ActivationOrder activationOrder = ActivationOrder.None;
        [SerializeField]
        private ColliderMode colliderMode = ColliderMode.None;

        //Settings
        [SerializeField]
        private bool DeactivateOnCollision = false;
        [SerializeField]
        private bool AlignTeleport = false;
        [SerializeField]
        private float ProximityDistance = 1;
        [SerializeField]
        private bool CreateObjects = false;
        [SerializeField]
        private bool AddTrigger = false;
        [SerializeField]
        private bool AddRigidbody = false;
        [SerializeField]
        private bool SingletonMode = false;
        [SerializeField]
        private bool LoopEnabled = false;
        //Private access to all entities
        private List<GameObject> entities = new List<GameObject>();

        //Entity Settings
        [SerializeField]
        private string TagEntity = "Untagged";
        [SerializeField]
        private GameObject SingleEntity = null;
        [SerializeField]
        private GameObject[] MultiEntity = { };

        //Events
        [SerializeField]
        ActivateEvent OnActivateEvent = new ActivateEvent();
        [SerializeField]
        DeactivateEvent OnDeactivateEvent = new DeactivateEvent();
        [SerializeField]
        TeleportEvent OnTeleportEvent = new TeleportEvent();
        [SerializeField]
        CollisionEvent OnCollisionEvent = new CollisionEvent();

        //Runtime fields
        private int mostRecentActive = -1;
        #endregion

        #region Runtime
        private void OnEnable()
        {
            ReloadEntities();
        }
        private void Start()
        {
            #if UNITY_EDITOR 
            GetRidOfWarnings(ShowAbsolutePosition);
            #endif
            //Triggers or prox checkers
            CreateObjectsInScene();
            //Checkpoint active state
            if (activationMode == ActivationMode.AlwaysActive)
            {
                EnableAll();
            }
            else if (SingletonMode)
            {
                SingletonInit();
            }
            SetMRA();
        }
        #endregion

        #region Private Methods
        private void GetRidOfWarnings(bool unused)
        {
            if (unused)
            {
                return;
            }
        }
        /// <summary>
        /// Create objects in the scene. More of a helper function
        /// </summary>
        private void CreateObjectsInScene()
        {
            //We only create the objects in the scene if they have triggerboxes. Otherwise its unncessary   
            if (activationMode == ActivationMode.Proximity || activationMode == ActivationMode.ActivateOnCollision || CreateObjects)
            {
                for (int i = 0; i < checkPoints.Count; i++)
                {
                    CreateCheckpointObject(i, checkPoints[i]);
                }
            }
        }
        /// <summary>
        /// Create an object for a checkpoint in the hierarchy.
        /// </summary>
        private void CreateCheckpointObject(int index, CheckPoint cp)
        {
            GameObject go = new GameObject("Checkpoint_" + index.ToString());
            go.transform.SetParent(transform);
            go.transform.position = cp.GetAbsolutePosition();
            if (activationMode == ActivationMode.Proximity)
            {
                go.AddComponent(typeof(ProximityChecker));
                go.GetComponent<ProximityChecker>().cpIndex = index;
                go.GetComponent<ProximityChecker>().distance = ProximityDistance;
            }
            if (activationMode == ActivationMode.ActivateOnCollision || AddTrigger)
            {
                go.AddComponent(typeof(BoxCollider));
                BoxCollider bc = go.GetComponent<BoxCollider>();
                CheckpointTrigger cpt = go.AddComponent(typeof(CheckpointTrigger)) as CheckpointTrigger;
                cpt.cpIndex = index;
                bc.isTrigger = true;
                bc.size = cp.GetBounds().size;
                bc.center = cp.GetBounds().center;
            }
            if (AddRigidbody)
            {
                go.AddComponent(typeof(Rigidbody));
                go.GetComponent<Rigidbody>().useGravity = false;
            }
        }
        /// <summary>
        /// [INTERNAL USE ONLY] Used to tell the system an entity has collided with a trigger volume.
        /// </summary>
        public void CollisionOccurred(int index, GameObject entity)
        {
            bool prevState = GetCheckpoint(index).GetActive();
            if (DeactivateOnCollision && prevState)
            {
                SetCheckpointState(index, false);
            }
            else if (activationMode == ActivationMode.ActivateOnCollision)
            {
                SetCheckpointState(index, true);
            }
            //Only invoke if criteria met
            if (colliderMode == ColliderMode.Both || (colliderMode == ColliderMode.ActiveCheckpoints && prevState)
                || (colliderMode == ColliderMode.InactiveCheckpoints && !prevState))
            {
                OnCollisionEvent.Invoke(index, entity);
            }
        }
        /// <summary>
        /// Teleport's an entity to a checkpoint
        /// </summary>
        private void TeleportEntityToCheckpoint(GameObject entity, CheckPoint cp)
        {
            OnTeleportEvent.Invoke(cp.GetIndex(), entity);
            entity.transform.position = cp.GetAbsolutePosition();
            if (AlignTeleport)
            {
                entity.transform.localEulerAngles = new Vector3(entity.transform.localEulerAngles.x, cp.GetDirection(), entity.transform.localEulerAngles.z);
            }
        }
        private bool CheckAllPreviousActive(int index)
        {
            for (int i = 0; i < index; i++)
            {
                if (!checkPoints[i].GetActive())
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Ensure on start there is only 1 checkpoint active. Used only for singleton order mode
        /// </summary>
        private void SingletonInit()
        {
            CheckPoint[] cpArr = GetActiveCheckpoints();
            if (cpArr.Length > 1)
            {
                for (int i = 1; i < cpArr.Length; i++)
                {
                    cpArr[i].SetActive(false);
                }
            }
        }
        /// <summary>
        /// Make sure that mostrecentactive is set on start if there are active checkpoints.
        /// </summary>
        private void SetMRA()
        {
            CheckPoint[] activePoints = GetActiveCheckpoints();
            if (mostRecentActive == -1 && activePoints.Length > 0)
            {
                mostRecentActive = activePoints[activePoints.Length-1].GetIndex();
            }
        }
        #endregion

        #region Public Methods
        //Teleporting to checkpoints
        /// <summary>
        /// Use only when a single entity is assigned to the system (in any mode).
        /// Teleports the entity to the nearest active checkpoint.
        /// </summary>
        public void TeleportToNearest()
        {
            if (entityMode == EntityMode.SingleEntity)
            {
                TeleportToNearest(SingleEntity);
                return;
            }
            else if (entities.Count == 1)
            {
                TeleportToNearest(entities[0]);
                return;
            }
            Debug.LogWarning("This method should only be used for single entity mode. See overloads for other modes.");
        }
        /// <summary>
        /// Teleports the specified entity to the nearest active checkpoint.
        /// </summary>
        public void TeleportToNearest(GameObject entity)
        {
            CheckPoint temp = GetNearestCheckpoint();
            if (temp != null)
            {
                TeleportEntityToCheckpoint(entity, temp);
            }
        }
        /// <summary>
        /// Use only when a single entity is assigned to the system (in any mode).
        /// Teleports the entity to checkpoint at index
        /// </summary>
        public void TeleportToCheckpoint(int index)
        {
            if (entityMode == EntityMode.SingleEntity)
            {
                TeleportToCheckpoint(index, SingleEntity);
                return;
            }
            else if (entities.Count == 1)
            {
                TeleportToCheckpoint(index, entities[0]);
                return;
            }
            Debug.LogWarning("This method should only be used for single entity mode. See overloads for other modes.");
        }
        /// <summary>
        /// Teleport entity to checkpoint with index.
        /// </summary>
        public void TeleportToCheckpoint(int index, GameObject entity)
        {
            CheckPoint temp = GetCheckpoint(index);
            if (temp != null)
            {
                TeleportEntityToCheckpoint(entity, temp);
            }
        }
        /// <summary>
        /// Use only when a single entity is assigned to the system (in any mode).
        /// Teleports the entity to the active checkpoint with the highest index.
        /// </summary>
        public void TeleportToLatest()
        {
            if (entityMode == EntityMode.SingleEntity)
            {
                TeleportToLatest(SingleEntity);
                return;
            }
            else if (entities.Count == 1)
            {
                TeleportToLatest(entities[0]);
                return;
            }
            Debug.LogWarning("This method should only be used for single entity mode. See overloads for other modes.");
        }
        /// <summary>
        /// Teleports the entity to the active checkpoint with the highest index.
        /// </summary>
        public void TeleportToLatest(GameObject entity)
        {
            CheckPoint temp = GetLatestCheckpoint();
            if (temp != null)
            {
                TeleportEntityToCheckpoint(entity, temp);
            }
        }
        /// <summary>
        /// Use only when a single entity is assigned to the system (in any mode).
        /// Teleports the entity to a random active checkpoint.
        /// </summary>
        public void TeleportToRandom()
        {
            if (entityMode == EntityMode.SingleEntity)
            {
                TeleportToRandom(SingleEntity);
                return;
            }
            else if (entities.Count == 1)
            {
                TeleportToRandom(entities[0]);
                return;
            }
            Debug.LogWarning("This method should only be used for single entity mode. See overloads for other modes.");
        }
        /// <summary>
        /// Teleports the entity to a random active checkpoint.
        /// </summary>
        public void TeleportToRandom(GameObject entity)
        {
            CheckPoint temp = GetRandomCheckpoint();
            if (temp != null)
            {
                TeleportEntityToCheckpoint(entity, temp);
            }
        }
        /// <summary>
        /// Use only when a single entity is assigned to the system (in any mode).
        /// Teleports the entity to the most recently activated checkpoint.
        /// </summary>
        public void TeleportToRecentlyActivated()
        {
            if (entityMode == EntityMode.SingleEntity)
            {
                TeleportToRecentlyActivated(SingleEntity);
                return;
            }
            else if (entities.Count == 1)
            {
                TeleportToRecentlyActivated(entities[0]);
                return;
            }
            Debug.LogWarning("This method should only be used for single entity mode. See overloads for other modes.");
        }
        /// <summary>
        /// Teleports the specified entity to the most recently activated checkpoint.
        /// </summary>
        public void TeleportToRecentlyActivated(GameObject entity)
        {
            CheckPoint temp = GetRecentlyActivatedCheckpoint();
            if (temp != null)
            {
                TeleportEntityToCheckpoint(entity, temp);
            }
        }
        //Getting checkpoints
        /// <summary>
        /// Returns the most recently activated checkpoint.
        /// </summary>
        public CheckPoint GetRecentlyActivatedCheckpoint()
        {
            if (GetActiveCheckpoints().Length <= 0)
            {
                Debug.LogWarning("No checkpoints have been activated yet.");
                return null;
            }
            if (mostRecentActive < 0)
            {
                return GetLatestCheckpoint();
            }
            return checkPoints[mostRecentActive];
        }
        /// <summary>
        /// Returns the activated checkpoint with the highest index.
        /// </summary>
        public CheckPoint GetLatestCheckpoint()
        {
            if (GetActiveCheckpoints().Length <= 0)
            {
                Debug.LogWarning("No checkpoints have been activated yet.");
                return null;
            }
            return GetActiveCheckpoints()[GetActiveCheckpoints().Length - 1];
        }
        /// <summary>
        /// Use only when a single entity is assigned to the system (in any mode). 
        /// This could be using SingleEntity mode, or having only a single entity assigned in Tag mode or MultipleEntity mode.<br></br>
        /// Returns the checkpoint closest to the assigned entity.
        /// </summary>
        public CheckPoint GetNearestCheckpoint()
        {
            if (entityMode == EntityMode.SingleEntity)
            {
                return GetNearestCheckpoint(SingleEntity.transform.position);
            }
            else if (entities.Count == 1)
            {
                return GetNearestCheckpoint(entities[0].transform.position);
            }
            Debug.LogWarning("This method should only be used in Single entity mode. To find the nearest checkpoint to a Vector3, see overloads.");
            return null;
        }
        /// <summary>
        /// Returns the nearest active checkpoint to Vector3
        /// </summary>
        public CheckPoint GetNearestCheckpoint(Vector3 position)
        {
            float smallestDistance = Vector3.Distance(position, checkPoints[0].GetAbsolutePosition());
            int index = 0;
            CheckPoint[] activeCP = GetActiveCheckpoints();

            for (int i = 0; i < activeCP.Length; i++)
            {
                float tempDist = Vector3.Distance(position, activeCP[i].GetAbsolutePosition());
                if (tempDist < smallestDistance)
                {
                    smallestDistance = tempDist;
                    index = i;
                }
            }
            return activeCP[index];
        }
        /// <summary>
        /// Returns a random active checkpoint
        /// </summary>
        public CheckPoint GetRandomCheckpoint()
        {
            return GetActiveCheckpoints()[Random.Range(0, GetActiveCheckpoints().Length)];
        }
        /// <summary>
        /// Returns the checkpoint at index
        /// </summary>
        public CheckPoint GetCheckpoint(int index)
        {
            return checkPoints[index];
        }
        /// <summary>
        /// Return all active checkpoints in an array
        /// </summary>
        public CheckPoint[] GetActiveCheckpoints()
        {
            return checkPoints.Where(cp => cp.GetActive() == true).ToArray();
        }
        /// <summary>
        /// Returns all inactive checkpoints in an array
        /// </summary>
        public CheckPoint[] GetInactiveCheckpoints()
        {
            return checkPoints.Where(cp => cp.GetActive() == false).ToArray();
        }
        //Adding checkpoints

        /// <summary>
        /// Add a checkpoint
        /// </summary>
        public CheckPoint AddCheckpoint(Vector3 pos, bool activeState, Space space = Space.Self)
        {
            //Add checkpoint to the list
            if (space == Space.World)
            {
                checkPoints.Add(new CheckPoint(pos - transform.position, activeState));
            }
            else
            {
                checkPoints.Add(new CheckPoint(pos, activeState));
            }
            return checkPoints[checkPoints.Count - 1];
        }
        /// <summary>
        /// Add a checkpoint
        /// </summary>
        public CheckPoint AddCheckpoint(Vector3 pos, bool activeState, Bounds bounds, Space space = Space.World)
        {
            //Add checkpoint to the list
            if (space == Space.World)
            {
                checkPoints.Add(new CheckPoint(pos - transform.position, activeState, bounds));
            }
            else
            {
                checkPoints.Add(new CheckPoint(pos, activeState, bounds));
            }
            return checkPoints[checkPoints.Count - 1];
        }
        //Seting checkpoint active states
        /// <summary>
        /// Set checkpoint's active state at 'index'
        /// </summary>
        public bool SetCheckpointState(int index, bool state)
        {
            if (checkPoints[index].GetActive() == state)
            {
                //Nothing to be done
                return false;
            }
            if (activationMode == ActivationMode.AlwaysActive && !state)
            {
                Debug.LogWarning("Activation State is 'Always Active', can not disable checkpoint.");
                return false;
            }

            //Activation Conditions
            if (state)
            {
                if (activationOrder == ActivationOrder.Sequential)
                {
                    //Sequential mode
                    if (LoopEnabled)
                    {
                        if (CheckAllPreviousActive(checkPoints.Count - 1))
                        {
                            SetCheckpointState(0, false);
                        }
                        else if (index == 0 && (GetActiveCheckpoints().Length == checkPoints.Count - 1 || (SingletonMode && GetCheckpointState(checkPoints.Count - 1))))
                        {
                            DisableAll();
                        }
                    }
                    //Check previous
                    if ((index != 0 && !GetCheckpointState(index - 1)) || (index == 0 && GetActiveCheckpoints().Length != 0))
                    {
                        return false;
                    }
                }
                if (SingletonMode)
                {
                    DisableAll();
                }

                //Set state
                mostRecentActive = index;
                checkPoints[index].SetActive(state);
                //Invoke event
                OnActivateEvent.Invoke(index);
            }

            //if switching to false
            else
            {
                //Set state
                checkPoints[index].SetActive(state);
                //Invoke event
                OnDeactivateEvent.Invoke(index);
            }
            return true;
        }
        /// <summary>
        /// Get the checkpoints state.
        /// </summary>
        public bool GetCheckpointState(int index)
        {
            return checkPoints[index].GetActive();
        }
        /// <summary>
        /// Disable all checkpoints
        /// </summary>
        public void DisableAll()
        {
            for (int i = 0; i < checkPoints.Count; i++)
            {
                if (checkPoints[i] != null)
                {
                    SetCheckpointState(i, false);
                }
            }
        }
        /// <summary>
        /// Enable all checkpoints
        /// </summary>
        public void EnableAll()
        {
            for (int i = 0; i < checkPoints.Count; i++)
            {
                if (checkPoints[i] != null)
                {
                    SetCheckpointState(i, true);
                }
            }
        }
        //Special case for when entities list is not up-to-date
        /// <summary>
        /// Only use if an entity has been destroyed or an entity has been spawned/destroyed in tag mode.<br></br>
        /// Reloads the list of entities currently using checkpoints. If in MultipleEntity mode, the list of entities is reset to its initial state.
        /// </summary>
        public void ReloadEntities()
        {
            if (entityMode == EntityMode.MultipleEntity)
            {
                entities = MultiEntity.Where(e => e != null).ToList();
            }
            else if (entityMode == EntityMode.SingleEntity)
            {
                if (SingleEntity == null)
                {
                    Debug.LogWarning("No entity detected. Assign entity in CheckpointController inspector.");
                }
            }
            else
            {
                entities = GameObject.FindGameObjectsWithTag(TagEntity).ToList();
            }
        }
        /// <summary>
        /// Only use in multipleEntity mode.<br></br>
        /// Adds gameobject to the list of entities currently using checkpoints and updates.
        /// </summary>
        public void AddNewEntity(GameObject entity)
        {
            if (entityMode == EntityMode.MultipleEntity)
            {
                entities.Add(entity);
            }
            else
            {
                Debug.LogWarning("This method can only be used in MultipleEntity Mode");
            }
        }
        /// <summary>
        /// Only use in multipleEntity mode.<br></br>
        /// Remove entity from list.
        /// </summary>
        public void RemoveEntity(GameObject entity)
        {
            if (entityMode == EntityMode.MultipleEntity)
            {
                if (!entities.Contains(entity))
                {
                    Debug.LogWarning("Entity not found.");
                    return;
                }
                entities.Remove(entity);
            }
            else
            {
                Debug.LogWarning("This method can only be used in MultipleEntity Mode");
            }
        }
        /// <summary>
        /// Get all entities configured to activate checkpoints.
        /// <br></br> Returns an array of gameobjects
        /// </summary>
        public GameObject[] GetEntities()
        {
            if (entityMode == EntityMode.SingleEntity)
            {
                return new GameObject[] { SingleEntity };
            }
            return entities.ToArray();
        }
        #endregion

        #region Gizmo
        #if UNITY_EDITOR
        //Draw gizmos for checkpoints
        private void OnDrawGizmos()
        {
            if (!HideGizmos)
            {
                GizmoDrawing();
            }
        }
        private void OnDrawGizmosSelected()
        {
            if (HideGizmos)
            {
                GizmoDrawing();
            }
        }
        private void GizmoDrawing()
        {
            for (int i = 0; i < checkPoints.Count; i++)
            {
                //Set Gizmo Color
                if (checkPoints[i].GetActive() == true)
                {
                    if (selectedIndices.Contains(i))
                    {
                        Gizmos.color = GlobalSettings.SelectedActiveColor;
                    }
                    else
                    {
                        Gizmos.color = GlobalSettings.ActiveColor;
                    }
                }
                else
                {
                    if (selectedIndices.Contains(i))
                    {
                        Gizmos.color = GlobalSettings.SelectedInactiveColor;
                    }
                    else
                    {
                        Gizmos.color = GlobalSettings.InactiveColor;
                    }
                }
                Gizmos.DrawSphere(checkPoints[i].GetAbsolutePosition(), GlobalSettings.CheckpointRadius * guiScale);
            }
        }
        #endif
        #endregion

        #region Editor Helper
        #if UNITY_EDITOR
        private void OnValidate()
        {
            //Whenever a new checkpoint is added to list, do stuff
            if (checkPoints.Count > checkPointCount)
            {
                //Set checkpoint reference
                checkPoints[checkPointCount].cpc = this;
                checkPoints[checkPointCount].SetBounds(new Bounds(Vector3.zero, Vector3.one));
            }
            else if (checkPoints.Count < checkPointCount)
            {
                selectedIndices.Clear();
            }
            checkPointCount = checkPoints.Count;
        }
        #endif
        #endregion
    }
}