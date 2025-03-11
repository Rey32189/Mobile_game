using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SmartPoint.Events
{
    [System.Serializable]
    public class CollisionEvent : UnityEvent<int, GameObject>
    {
    }
    [System.Serializable]
    public class ActivateEvent : UnityEvent<int>
    {
    }
    [System.Serializable]
    public class DeactivateEvent : UnityEvent<int>
    {
    }
    [System.Serializable]
    public class TeleportEvent : UnityEvent<int, GameObject>
    {
    }
    [System.Serializable]
    public class SpawnEvent : UnityEvent<GameObject, int>
    {
    }
    [System.Serializable]
    public class ExitEvent : UnityEvent
    {

    }
}
