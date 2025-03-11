using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using SmartPoint.Utility;

namespace SmartPoint.Editors {

    [CustomEditor(typeof(CheckPointController)), CanEditMultipleObjects]
    public class CheckPointHandleEditor : Editor
    {
        //For editor
        CheckPointController cpc;
        BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();
        GUIStyle numberLabel;

        //For inspector
        GUIStyle TopNote = new GUIStyle();
        private bool HideDirectionHandle = false;

        #region Serialized Properties
        SerializedProperty hideGizmosProp;
        SerializedProperty absPositionsProp;
        SerializedProperty cpCountProp;
        SerializedProperty alignTeleportProp;
        SerializedProperty singletonProp;
        SerializedProperty loopProp;
        SerializedProperty guiScaleProp;
        SerializedProperty checkpointListProp;
        SerializedProperty createObjProp;
        //Enums
        SerializedProperty colDisabledProp;
        SerializedProperty activationModeProp;
        SerializedProperty entityModeProp;
        SerializedProperty activeOrderProp;
        SerializedProperty colModeProp;
        //Mode sub-fields
        SerializedProperty singleEntityProp;
        SerializedProperty multiEntityProp;
        SerializedProperty tagEntityProp;
        SerializedProperty distanceProp;
        SerializedProperty addObjTriggerProp;
        SerializedProperty addObjRigidbodyProp;
        //Events
        SerializedProperty activateEventProp;
        SerializedProperty deactiveEventProp;
        SerializedProperty teleportEventProp;
        SerializedProperty collisionEventProp;

        #endregion

        #region Inspector Labels
        bool modesSettings = true;
        bool activationSettings = true;
        bool editorSettings = true;
        bool eventSettings = true;

#if !UNITY_2020_3_OR_NEWER
    bool cpSettings = true;
#endif
        #endregion

        #region GUIContent
        //Entity Mode
        GUIContent entityModeGUI = new GUIContent("Entity Mode",
                    "Determines whether out of bounds teleportation will occur for a single entity or multiple." +
                    "\nFor Player configuration, select Single Entity or Tag.");
        GUIContent tagModeGUI = new GUIContent("Tag", "All entities with the input tag will be bound to the checkpoint system.");
        GUIContent singleEntityGUI = new GUIContent("Entity", "The entity bound to the checkpoint system. Typically player.");
        GUIContent multiEntityGUI = new GUIContent("Entities", "The number of entities which will be bound to the checkpoint system");
        //Activation Mode
        GUIContent activationModeGUI = new GUIContent("Activation Mode", "Specifies the activation state/criteria for the checkpoints." +
            "\n\nManualActivation: Checkpoints will only activate when manually set in inspector or via script. See docs for more information." +
            "\n\nAlways Active: Checkpoints will always be active." +
            "\n\nActivateOnCollision: Checkpoints will activate when entity enters bounds." +
            "\n\nProximity: Checkpoints will activate when entities are within radial distance.");
        GUIContent proxModeGUI = new GUIContent("Distance", "Maximum distance at which checkpoint will activate.");
        //Activation Order
        GUIContent activationOrderGUI = new GUIContent("Activation Order", "This is what conditions apply to checkpoint activation." +
            "\n\nSequential: A checkpoint will only activate if the previous checkpoint has been activated.");
        //Collision Mode
        GUIContent colModeGUI = new GUIContent("Collision Mode", "This is only used for the OnCollision Event.\nSpecifies which checkpoints the callbacks will trigger for.");

        //Editor Settings
        GUIContent hideGizmoGUI = new GUIContent("Hide Gizmos", "Checkpoints will only be visible when the controller is selected in the hierarchy.");
        GUIContent showAbsPosGUI = new GUIContent("Absolute Position", "Checkpoint positions will be absolute instead of relative in the inspector.");
        //Activation Settings
        GUIContent colDisabledGUI = new GUIContent("Deactivate on Collision",
                "Colliding with a checkpoint's trigger box will disable the checkpoint if it is active.\nIt will still enable like normal.");
        GUIContent alignTeleGUI = new GUIContent("Align Entity Teleport", "If true, on teleport, entities y axis rotation will align with the checkpoint.");
        GUIContent createObjGUI = new GUIContent("Create Objects in Scene",
                    "If true, Gameobjects with trigger colliders will be created in the scene at runtime.\n\n" +
                    "Note: This is always true if ActivationMode is set to ActivateOnCollision or Proximity.");
        GUIContent addTriggerGUI = new GUIContent("Add Trigger Collider", "Add trigger collider to checkpoint object in scene. Used for OnCollision Event.");
        GUIContent addRigidGUI = new GUIContent("Add Rigidbody",
                    "Add Rigidbody to checkpoint object in scene.\nGravity is disabled by default.\n\n" +
                    "Used for OnCollision Event, since one rigidbody must be present for collisions.");
        GUIContent singletonGUI = new GUIContent("Singleton", "Only one checkpoint can be active at a time.");
        GUIContent loopGUI = new GUIContent("Looping", "Once all checkpoints have been activated, all checkpoints are set to inactive. Checkpoints can also only be activated in sequence.\n\n" +
            "Note: All checkpoints only disable once the final checkpoint is triggered, " +
            "however the final checkpoint is only disabled once the first checkpoint is activated once again.");
        #endregion

        void OnEnable()
        {
            #region Serialized Properties
            //Find all properties set above
            cpc = serializedObject.targetObject as CheckPointController;

            hideGizmosProp = serializedObject.FindProperty("HideGizmos");
            absPositionsProp = serializedObject.FindProperty("ShowAbsolutePosition");
            colDisabledProp = serializedObject.FindProperty("DeactivateOnCollision");
            activationModeProp = serializedObject.FindProperty("activationMode");
            entityModeProp = serializedObject.FindProperty("entityMode");
            cpCountProp = serializedObject.FindProperty("checkPointCount");
            alignTeleportProp = serializedObject.FindProperty("AlignTeleport");
            activeOrderProp = serializedObject.FindProperty("activationOrder");
            singleEntityProp = serializedObject.FindProperty("SingleEntity");
            multiEntityProp = serializedObject.FindProperty("MultiEntity");
            tagEntityProp = serializedObject.FindProperty("TagEntity");
            distanceProp = serializedObject.FindProperty("ProximityDistance");
            checkpointListProp = serializedObject.FindProperty("checkPoints");
            createObjProp = serializedObject.FindProperty("CreateObjects");
            colModeProp = serializedObject.FindProperty("colliderMode");
            addObjRigidbodyProp = serializedObject.FindProperty("AddRigidbody");
            addObjTriggerProp = serializedObject.FindProperty("AddTrigger");
            singletonProp = serializedObject.FindProperty("SingletonMode");
            loopProp = serializedObject.FindProperty("LoopEnabled");
            guiScaleProp = serializedObject.FindProperty("guiScale");

            //Events
            activateEventProp = serializedObject.FindProperty("OnActivateEvent");
            deactiveEventProp = serializedObject.FindProperty("OnDeactivateEvent");
            teleportEventProp = serializedObject.FindProperty("OnTeleportEvent");
            collisionEventProp = serializedObject.FindProperty("OnCollisionEvent");
            #endregion

            #region GUIStyles
            //Define GUIStyle on start
            numberLabel = new GUIStyle();
            numberLabel.fontStyle = FontStyle.Bold;
            numberLabel.alignment = TextAnchor.MiddleCenter;
            TopNote.fontStyle = FontStyle.Italic;
            if (EditorGUIUtility.isProSkin) {
                TopNote.normal.textColor = new Color(0.75f, 0.75f, 0.75f);
            }
            else {
                TopNote.normal.textColor = Color.black;
            }
            #endregion
        }

        private void OnDisable()
        {
            if (cpc != null)
            {
                ClearIndices();
            }
        }

        void OnSceneGUI()
        {
            #region Variables
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);

            Camera cam = SceneView.currentDrawingSceneView.camera;
            float camFOV = cam.fieldOfView;
            float screenHeight = cam.pixelHeight;
            bool remNode = false;
            float rotationDiff = 0f;
            #endregion

            #region Input
            numberLabel.normal.textColor = Color.black;
            
            for (int i = 0; i < cpc.checkPoints.Count; i++)
            {
                // Rough estimation of whether a gizmo sphere is on screen or not, using the center of sphere as a respresentation of the whole
                if (DisplayUtility.PointOnScreen(cam, cpc.checkPoints[i].GetAbsolutePosition()))
                {
                    //Determine if mouse is hovering over gizmo. If not hovering no point in checking for anything
                    bool overlap = DisplayUtility.SphereMouseOverlap(cpc.checkPoints[i].GetAbsolutePosition(), GlobalSettings.CheckpointRadius * guiScaleProp.floatValue, cam, camFOV, screenHeight);
                    if (overlap)
                    {
                        switch (current.type)
                        {
                            //If left click
                            case EventType.MouseDown:
                                if (!Event.current.control)
                                {
                                    if (current.button == 1)
                                    {
                                        RemoveFromIndexList(i);
                                    }
                                    ClearIndices();
                                    AddToIndexList(i);
                                    break;
                                }
                                if (!cpc.selectedIndices.Contains(i))
                                {
                                    AddToIndexList(i);
                                    break;
                                }
                                RemoveFromIndexList(i);
                                break;
                            case EventType.MouseUp:
                                if (Event.current.control)
                                {
                                    if (current.button == 1)
                                    {
                                        RemoveFromIndexList(i);
                                        RemoveCheckpoint(i);
                                        remNode = true;
                                    }
                                }
                                break;
                            case EventType.KeyDown:
                                if (Event.current.control)
                                {
                                    numberLabel.normal.textColor = Color.white;
                                }
                                break;
                            case EventType.KeyUp:
                                break;
                            case EventType.Layout:
                                HandleUtility.AddDefaultControl(controlID);
                                break;
                        }
                    }

                    //If a checkpoint to remove has been selected, break out of loop to avoid invalid index (since we're changing the list count)
                    if (remNode)
                    {
                        break;
                    }
                    //Still if on screen and within range of 5 unity units, draw label
                    numberLabel.fontSize = 24;
                    if (DisplayUtility.IsSceneViewCameraInRange(cam, cpc.checkPoints[i].GetAbsolutePosition(), GlobalSettings.LabelRenderDistance))
                    {
                        Handles.Label(DisplayUtility.CalculateLabelPosition(cam, cpc.checkPoints[i].GetAbsolutePosition(), numberLabel, i.ToString()), i.ToString(), numberLabel);
                    }
                }
            }

            #endregion

            #region Display
            //If a checkpoint has been selected
            foreach (int index in cpc.selectedIndices)
            {
                //Begin record for undo
                EditorGUI.BeginChangeCheck();

                //update checkpoint position
                Vector3 positionDiff = Handles.PositionHandle(cpc.checkPoints[index].GetAbsolutePosition(), Quaternion.identity) - cpc.checkPoints[index].GetAbsolutePosition();

                if (!HideDirectionHandle)
                {
                    rotationDiff = DrawDirectionHandle(index);
                    DrawDirectionLine(index);
                }

                Vector3 sizeDiff = DrawColliderHandle(index);

                //Apply undo if necessary
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed Position with handles");
                    //Update position for all selected checkpoints
                    foreach (int i in cpc.selectedIndices)
                    {
                        //Updated Position
                        cpc.checkPoints[i].SetPosition(cpc.checkPoints[i].GetPosition() + positionDiff);

                        //Updated Rotation
                        cpc.checkPoints[i].SetDirection(cpc.checkPoints[i].GetDirection() + rotationDiff);

                        //Updated Bounds
                        //Bounds handle
                        if ((ActivationMode)activationModeProp.enumValueIndex == ActivationMode.ActivateOnCollision || addObjTriggerProp.boolValue)
                        {
                            cpc.checkPoints[i].SetBoundsSize(cpc.checkPoints[i].GetBoundsSize() + sizeDiff);
                        }
                    }
                }
            }
            //Gets rid of input lag
            SceneView.RepaintAll();
            #endregion
        }

        #region Draw Helpers
        /// <summary>
        /// Draws the handle for direction of checkpoint. 
        /// Returns the direction difference (amount of change)
        /// </summary>
        private float DrawDirectionHandle(int dirIndex)
        {
            //Update checkpoint direction
            Handles.color = GlobalSettings.DirectionCircleColor;
            return Handles.Disc(Quaternion.Euler(0f, cpc.checkPoints[dirIndex].GetDirection(), 0f), cpc.checkPoints[dirIndex].GetAbsolutePosition(),
                Vector3.up, GlobalSettings.DirectionHandleRadius, false, 0f).eulerAngles.y - cpc.checkPoints[dirIndex].GetDirection();
        }

        private void DrawDirectionLine(int lineIndex)
        {
            //Draw arrow for direction
            Handles.color = GlobalSettings.DirectionLineColor;
            float z = GlobalSettings.DirectionHandleRadius * Mathf.Cos(cpc.checkPoints[lineIndex].GetDirection() * Mathf.Deg2Rad);
            float x = GlobalSettings.DirectionHandleRadius * Mathf.Sin(cpc.checkPoints[lineIndex].GetDirection() * Mathf.Deg2Rad);
            #if (UNITY_2020_2_OR_NEWER)
            Handles.DrawLine(cpc.checkPoints[lineIndex].GetAbsolutePosition(), cpc.checkPoints[lineIndex].GetAbsolutePosition() + new Vector3(x, 0f, z), 2f);
            #else
            Handles.DrawLine(cpc.checkPoints[lineIndex].GetAbsolutePosition(), cpc.checkPoints[lineIndex].GetAbsolutePosition() + new Vector3(x, 0f, z));
            #endif
        }

        private Vector3 DrawColliderHandle(int colIndex)
        {
            //If collider mode, display collider handles
            if ((ActivationMode)activationModeProp.enumValueIndex == ActivationMode.ActivateOnCollision || addObjTriggerProp.boolValue)
            {
                m_BoundsHandle.center = cpc.checkPoints[colIndex].GetBounds().center + cpc.checkPoints[colIndex].GetAbsolutePosition();
                m_BoundsHandle.size = cpc.checkPoints[colIndex].GetBounds().size;
                Handles.color = GlobalSettings.WireFrameColor;
                m_BoundsHandle.DrawHandle();
                return m_BoundsHandle.size - cpc.checkPoints[colIndex].GetBounds().size;
            }
            return Vector3.zero;
        }
        #endregion

        #region List Helpers
        private void AddToIndexList(int i)
        {
            if (!cpc.selectedIndices.Contains(i))
            {
                Undo.RegisterCompleteObjectUndo(cpc, "New checkpoint selected. " + cpc.gameObject.name);
                Undo.FlushUndoRecordObjects();
                //Set which checkpoint is being selected in controller
                cpc.selectedIndices.Add(i);
                EditorUtility.SetDirty(cpc);
            }
        }
        private void RemoveFromIndexList(int i)
        {
            if (cpc.selectedIndices.Contains(i))
            {
                Undo.RegisterCompleteObjectUndo(cpc, "Selected checkpoint removed. " + cpc.gameObject.name);
                Undo.FlushUndoRecordObjects();
                //Set which checkpoint is being selected in controller
                cpc.selectedIndices.Remove(i);
                EditorUtility.SetDirty(cpc);
            }
        }
        private void RemoveCheckpoint(int i)
        {
            cpCountProp.intValue -= 1;
            cpc.checkPoints.RemoveAt(i);
            //Update selectedindices list since indexes might change now
            for (int rem = 0; rem < cpc.selectedIndices.Count; rem++)
            {
                if (cpc.selectedIndices[rem] > i)
                {
                    cpc.selectedIndices[rem] -= 1;
                }
            }
        }
        //Empty List
        private void ClearIndices()
        {
            if (cpc.selectedIndices.Count > 0)
            {
                Undo.RegisterCompleteObjectUndo(cpc, "Selected Checkpoints cleared. " + cpc.gameObject.name);
                Undo.FlushUndoRecordObjects();
                cpc.selectedIndices.Clear();
                EditorUtility.SetDirty(cpc);
            }
        }

    //TODO: After unit testing, remove in next update
    #if !UNITY_2020_3_OR_NEWER
    private void ShowSerializedList(SerializedProperty list, bool showListSize = true) {
        cpSettings = EditorGUILayout.BeginFoldoutHeaderGroup(cpSettings, "Checkpoints");
        EditorGUI.indentLevel += 1;
		if (cpSettings) {
			if (showListSize) {
				EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
			}
            EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++) {
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), true);
			}
            EditorGUI.indentLevel -= 1;
        }
		EditorGUI.indentLevel -= 1;
        EditorGUILayout.EndFoldoutHeaderGroup();
	}
    #endif
        #endregion

        #region Inspector Helpers
        private void DisplayTopNote()
        {
            //Note at top
            EditorGUILayout.LabelField("Left Click to select a checkpoint", TopNote);
            EditorGUILayout.LabelField("Ctrl + Left Click to select multiple checkpoints", TopNote);
            EditorGUILayout.LabelField("Ctrl + Right Click to delete a checkpoint", TopNote);
        }
        private void DisplayCheckpointList()
        {
            //Checkpoints list
            #if UNITY_2020_3_OR_NEWER
            EditorGUILayout.PropertyField(checkpointListProp, new GUIContent("Checkpoints", "List of all checkpoints."), true);
            #else
            //ShowSerializedList(checkpointListProp); //Deprecated
            EditorGUILayout.PropertyField(checkpointListProp, new GUIContent("Checkpoints", "List of all checkpoints."), true);
            #endif
        }
        private void DisplayModeSettings()
        {
            EditorGUI.indentLevel = 0;
            modesSettings = EditorGUILayout.BeginFoldoutHeaderGroup(modesSettings, "Modes");
            if (modesSettings)
            {
                //Entity Mode//
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(entityModeProp, entityModeGUI);
                EditorGUILayout.Space(0.5f);
                EditorGUI.indentLevel = 2;
                EditorGUIUtility.labelWidth += GlobalSettings.indentField;
                //Tag Mode
                if (entityModeProp.enumValueIndex == (int)EntityMode.TagMode)
                {
                    tagEntityProp.stringValue = EditorGUILayout.TagField(tagModeGUI, tagEntityProp.stringValue);
                }
                //Single Entity Mode
                else if (entityModeProp.enumValueIndex == (int)EntityMode.SingleEntity)
                {
                    EditorGUILayout.PropertyField(singleEntityProp, singleEntityGUI);
                }
                //MultipleEntity Mode
                else
                {
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    EditorGUILayout.PropertyField(multiEntityProp, multiEntityGUI);
                }
                EditorGUIUtility.labelWidth -= GlobalSettings.indentField;

                //Activation Mode//
                EditorGUI.indentLevel = 1;
                EditorGUILayout.Space(0.1f);
                EditorGUILayout.PropertyField(activationModeProp, activationModeGUI);
                EditorGUI.indentLevel = 2;
                EditorGUIUtility.labelWidth += GlobalSettings.indentField;
                //Proximity Mode
                if (activationModeProp.enumValueIndex == (int)ActivationMode.Proximity)
                {
                    EditorGUILayout.PropertyField(distanceProp, proxModeGUI);
                }
                EditorGUIUtility.labelWidth -= GlobalSettings.indentField;

                if (activationModeProp.enumValueIndex != (int)ActivationMode.AlwaysActive)
                {
                    //Order Mode//
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.Space(0.2f);
                    EditorGUILayout.PropertyField(activeOrderProp, activationOrderGUI);
                }

                //Collider Mode//
                EditorGUI.indentLevel = 1;
                EditorGUILayout.Space(0.2f);
                EditorGUILayout.PropertyField(colModeProp, colModeGUI);

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.indentLevel = 0;
        }
        private void DisplayActivationSettings()
        {
            activationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(activationSettings, "Activation Settings");
            if (activationSettings)
            {
                EditorGUI.indentLevel = 1;
                if (singletonProp.boolValue)
                {
                    GUI.enabled = false;
                    colDisabledProp.boolValue = false;
                    EditorGUILayout.PropertyField(colDisabledProp, colDisabledGUI);
                    GUI.enabled = true;
                }
                else
                {
                    EditorGUILayout.PropertyField(colDisabledProp, colDisabledGUI);
                }
                EditorGUILayout.PropertyField(alignTeleportProp, alignTeleGUI);
                EditorGUILayout.PropertyField(createObjProp, createObjGUI);

                if (createObjProp.boolValue)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUIUtility.labelWidth += GlobalSettings.boolIndentField;
                    //Add Trigger
                    EditorGUILayout.PropertyField(addObjTriggerProp, addTriggerGUI);
                    //Add Rigidbody
                    EditorGUILayout.PropertyField(addObjRigidbodyProp, addRigidGUI);
                    EditorGUIUtility.labelWidth -= GlobalSettings.boolIndentField;
                    EditorGUI.indentLevel = 1;
                }

                if (activationModeProp.enumValueIndex == (int)ActivationMode.AlwaysActive)
                {
                    GUI.enabled = false;
                    EditorGUILayout.Toggle(singletonGUI, false);
                    EditorGUILayout.Toggle(loopGUI, false);
                }
                else if (activeOrderProp.enumValueIndex != (int)ActivationOrder.Sequential)
                {
                    EditorGUILayout.PropertyField(singletonProp, singletonGUI);
                    GUI.enabled = false;
                    EditorGUILayout.Toggle(loopGUI, false);
                }
                else
                {
                    EditorGUILayout.PropertyField(singletonProp, singletonGUI);
                    EditorGUILayout.PropertyField(loopProp, loopGUI);
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.indentLevel = 0;
        }
        private void DisplayEditorSettings()
        {
            editorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(editorSettings, "Editor");
            if (editorSettings)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(hideGizmosProp, hideGizmoGUI);
                HideDirectionHandle = EditorGUILayout.Toggle(new GUIContent("Hide Direction Handle", "Hide the direction handle when a checkpoint is selected in scene view."), HideDirectionHandle);
                EditorGUILayout.PropertyField(absPositionsProp, showAbsPosGUI);
                guiScaleProp.floatValue = EditorGUILayout.Slider(new GUIContent("GUI Scale", "The size of the Scene GUI."), guiScaleProp.floatValue, 1, 8);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.indentLevel = 0;
        }
        private void DisplayEventSettings()
        {
            eventSettings = EditorGUILayout.BeginFoldoutHeaderGroup(eventSettings, "Events");
            if (eventSettings)
            {
                EditorGUILayout.Space(1);
                EditorGUILayout.PropertyField(activateEventProp, new GUIContent("OnActivate", "Triggers when a checkpoint activates."));
                EditorGUILayout.Space(0.5f);
                EditorGUILayout.PropertyField(deactiveEventProp, new GUIContent("OnDeactivate", "Triggers when a checkpoint deactivates."));
                EditorGUILayout.Space(0.5f);
                EditorGUILayout.PropertyField(teleportEventProp, new GUIContent("OnTeleport", "Triggers when an entity teleports."));
                EditorGUILayout.Space(0.5f);
                #if !Unity_2020_3_OR_NEWER
                //if(Event.current.type == EventType.Repaint) {
                    EditorGUILayout.PropertyField(collisionEventProp, new GUIContent("OnCollision", "Triggers when an entity enters a checkpoints trigger box."));
                //}
                #else
                EditorGUILayout.PropertyField(collisionEventProp, new GUIContent("OnCollision", "Triggers when an entity enters a checkpoints trigger box."));
                #endif
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.indentLevel = 0;
        }
        private void UpdateCheckPointRefs()
        {
            for(int i = 0; i < cpc.checkPoints.Count; i++)
            {
                if(cpc.checkPoints[i].cpc == null)
                {
                    cpc.checkPoints[i].cpc = cpc;
                }
            }
        }
        #endregion

        public override void OnInspectorGUI()
        {
            #if !UNITY_2020_3_OR_NEWER
            UpdateCheckPointRefs();
            #endif
            DisplayTopNote();

            //Start custom inspector
            serializedObject.Update();

            DisplayCheckpointList();

            DisplayModeSettings();

            DisplayActivationSettings();

            DisplayEditorSettings();

            DisplayEventSettings();

            //End of inspector
            serializedObject.ApplyModifiedProperties();
        }
    }
}
