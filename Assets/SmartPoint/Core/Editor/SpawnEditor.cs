using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using SmartPoint.Utility;

namespace SmartPoint.Editors
{
    [CustomEditor(typeof(SpawnController)), CanEditMultipleObjects]
    public class SpawnEditor : Editor
    {
        BoxBoundsHandle m_BoxBounds = new BoxBoundsHandle();

        private bool onSameObject = false;
        private bool modeSettings = true;
        private bool editorSettings = true;
        private bool otherSettings = true;
        private bool eventSettings = true;

        #region Serialized Properties
        CheckPointController cpc;
        SpawnController sc;
        SerializedProperty cpcProp;
        //conditional vars
        SerializedProperty circleRadProp;
        SerializedProperty sphereRadiusProp;
        SerializedProperty rectSizeProp;
        SerializedProperty spawnDelayProp;
        SerializedProperty atsProp;
        SerializedProperty refillDelayProp;
        SerializedProperty enableRefillMaxProp;
        SerializedProperty enableSpawnMaxProp;
        SerializedProperty refillMaxProp;
        SerializedProperty constMaxProp;
        SerializedProperty exitTimeProp;
        SerializedProperty exitSpawnCountProp;
        SerializedProperty targetEntityProp;
        SerializedProperty customDirectionProp;
        SerializedProperty includeYPlaneProp;
        SerializedProperty rectPrismSizeProp;
        //Enums
        SerializedProperty spawnModeProp;
        SerializedProperty spawnAreaProp;
        SerializedProperty spawnLocationProp;
        SerializedProperty spawnDirectionProp;
        SerializedProperty exitConditionProp;
        //
        SerializedProperty offsetProp;
        //Editor settings
        SerializedProperty hideGizmosProp;
        SerializedProperty checkForColProp;
        SerializedProperty equalSpawnChanceProp;
        //Events
        SerializedProperty onSpawnProp;
        SerializedProperty onExitProp;
        //Lists
        SerializedProperty entityPoolProp;
        #endregion

        #region GUIContent
        GUIContent cpcGUI = new GUIContent("Checkpoint Controller", "This property only appears if your SpawnController is on a different " +
            "object than your CheckpointController.");
        GUIContent circleGUI = new GUIContent("Circle Radii", "Entities will only spawn between the radii.");
        GUIContent spawnAreaGUI = new GUIContent("Spawn Area", "Speicifies the area that entities from the Entity Pool will spawn in.\n\n" +
            "For circle mode, entities will spawn between the two radii (outside the red circle, but within the green).");
        GUIContent spawnModeGUI = new GUIContent("Spawn Mode", "Manual: Spawn() must be called through custom code to spawn entities." +
            "\nOneTimeSpawn: Spawns specified number of entities in a burst at runtime." +
            "\nConstant: Spawns entities at a regular rate." +
            "\nRefill: Only one entity will spawn at each checkpoint. When a checkpoint's assigned entity is destroyed, another will spawn after <refill delay> seconds." +
            "\n\nSee Manual for more info on use cases.");
        GUIContent spawnLocationGUI = new GUIContent("Spawn Location", "HighestIndex: Select the checkpoint with the highest index." +
            "\nMostRecentlyActivated: Select the most recently activated checkpoint (if its still active)." +
            "\nNearest: Select the nearest checkpoint to target entity." +
            "\nRandom: Select a random active checkpoint.");
        GUIContent spawnDirectionGUI = new GUIContent("Spawn Direction", "The direction the spawned entity will be facing.");
        GUIContent spawnDelayGUI = new GUIContent("Spawn Delay", "Time (in seconds) between each spawn.");
        GUIContent atsGUI = new GUIContent("Amount To Spawn", "The number of entities to spawn.");
        GUIContent refillDelayGUI = new GUIContent("Refill Delay", "Once previously spawned entity has been destroyed, " +
            "spawn another after <input> seconds.");
        GUIContent sphereRadiusGUI = new GUIContent("Sphere Radius", "Radius of the spawn area.");
        GUIContent rectSizeGUI = new GUIContent("Rect Size", "Rectangle Width and Height.");
        GUIContent hideGizmosGUI = new GUIContent("Hide Gizmos", "Hide all gizmos.");
        GUIContent rectPrismGUI = new GUIContent("Prism Size", "Length, Width and Height of spawn area.");
        GUIContent checkForColGUI = new GUIContent("Check For Collision", "Only spawn an entity if the location is clear. This is expensive and may increase spawn delays.");
        GUIContent enableMaxGUI = new GUIContent("Enable Maximum", "If true, only the specified number of entities can exist at one time.");
        GUIContent spawnMaxGUI = new GUIContent("Max Count", "The maximum number of entities that can exist at once.");
        GUIContent entityPoolGUI = new GUIContent("Entity Pool", "List of all entities that can spawn.");
        GUIContent offsetGUI = new GUIContent("Offset", "Spawn area offset from checkpoint.");
        GUIContent equalSpawnChanceGUI = new GUIContent("Equal Spawn Chance", "The chance to spawn is the same for all entities.");
        GUIContent exitConditionGUI = new GUIContent("Exit Condition", "Spawn Controller will permanently stop spawning entities if condition is met.");
        GUIContent exitTimeGUI = new GUIContent("Exit Time", "After <time> seconds, entities will permanently stop spawning.");
        GUIContent exitSpawnCountGUI = new GUIContent("Exit Spawn Count", "Once <count> entities have spawned, entities will permanently stop spawning.");
        GUIContent targetEntityGUI = new GUIContent("Target Entity", "Used for 'Face Entity' and 'Nearest to Entity'");
        GUIContent customDirectionGUI = new GUIContent("Direction", "Entity will spawn facing this direction.");
        GUIContent includeYPlaneGUI = new GUIContent("Include Y Plane", "The spawned entity will orient themselves to face the target entity above and below as well.");

        #endregion

        private void OnEnable()
        {
            cpcProp = serializedObject.FindProperty("CP_Controller");
            if (((MonoBehaviour)serializedObject.targetObject).TryGetComponent(out CheckPointController tempcpc)) {
                onSameObject = true;
                cpc = tempcpc;
                cpcProp.objectReferenceValue = tempcpc;
                serializedObject.ApplyModifiedProperties();
            }
            else {
                onSameObject = false;
            }
            sc = serializedObject.targetObject as SpawnController;

            #region Serialized Properties
            //Area vars
            circleRadProp = serializedObject.FindProperty("circleRadius");
            sphereRadiusProp = serializedObject.FindProperty("sphereRadius");
            rectSizeProp = serializedObject.FindProperty("rectSize");
            //Enums
            spawnModeProp = serializedObject.FindProperty("spawnMode");
            spawnAreaProp = serializedObject.FindProperty("spawnArea");
            spawnLocationProp = serializedObject.FindProperty("spawnLocation");
            spawnDirectionProp = serializedObject.FindProperty("spawnDirection");
            exitConditionProp = serializedObject.FindProperty("exitCondition");
            //Condition vars
            offsetProp = serializedObject.FindProperty("offsetSpawn");
            refillDelayProp = serializedObject.FindProperty("refillDelay");
            spawnDelayProp = serializedObject.FindProperty("spawnDelay");
            atsProp = serializedObject.FindProperty("amountToSpawn");
            enableRefillMaxProp = serializedObject.FindProperty("enableRefillMax");
            enableSpawnMaxProp = serializedObject.FindProperty("enableSpawnMax");
            refillMaxProp = serializedObject.FindProperty("refillMax");
            constMaxProp = serializedObject.FindProperty("constSpawnMax");
            exitTimeProp = serializedObject.FindProperty("exitTime");
            exitSpawnCountProp = serializedObject.FindProperty("exitSpawnCount");
            targetEntityProp = serializedObject.FindProperty("targetEntity");
            customDirectionProp = serializedObject.FindProperty("customDirection");
            includeYPlaneProp = serializedObject.FindProperty("includeYPlane");
            rectPrismSizeProp = serializedObject.FindProperty("rectPrismSize");
            //Editor
            hideGizmosProp = serializedObject.FindProperty("hideGizmos");
            checkForColProp = serializedObject.FindProperty("checkForCollision");
            equalSpawnChanceProp = serializedObject.FindProperty("equalSpawnChance");
            //Event
            onSpawnProp = serializedObject.FindProperty("onSpawnEvent");
            onExitProp = serializedObject.FindProperty("onExitEvent");
            //List
            entityPoolProp = serializedObject.FindProperty("entityPool");

            #endregion
        }

        private void OnSceneGUI()
        {
            if (cpc == null)
            {
                cpc = cpcProp.objectReferenceValue as CheckPointController;
                return;
            }
            for (int i = 0; i < cpc.checkPoints.Count; i++)

            {
                if (!hideGizmosProp.boolValue)
                {
                    Vector3 cpPos = cpc.checkPoints[i].GetAbsolutePosition() + offsetProp.vector3Value;

                    //Begin record for undo
                    EditorGUI.BeginChangeCheck();

                    if (spawnAreaProp.enumValueIndex == (int)SpawnArea.Circle)
                    {
                        DrawCircle(cpPos, circleRadProp.vector2Value.x, circleRadProp.vector2Value.y, i);
                    }
                    else if (spawnAreaProp.enumValueIndex == (int)SpawnArea.Rect)
                    {
                        DrawRectangle(cpPos, rectSizeProp.vector2Value, i);
                    }
                    else if (spawnAreaProp.enumValueIndex == (int)SpawnArea.RectPrism)
                    {
                        DrawRectPrism(cpPos, i);
                    }
                    //Apply undo if necessary
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Changed Position with handles");
                        if (spawnAreaProp.enumValueIndex == (int)SpawnArea.RectPrism)
                        {
                            sc.SetRectPrismSize(m_BoxBounds.size);
                        }
                    }
                }
            }
            SceneView.RepaintAll();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (!onSameObject)
            {
                EditorGUILayout.PropertyField(cpcProp, cpcGUI);
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.PropertyField(entityPoolProp, entityPoolGUI);
            EditorGUILayout.Space(1);

            if (!MathUtility.SumTo100(sc.GetSpawnChances()) && !equalSpawnChanceProp.boolValue)
            {
                EditorGUILayout.HelpBox("Spawn chances do not sum to 100.", MessageType.Warning);
            }

            //Modes
            EditorGUI.indentLevel = 0;
            modeSettings = EditorGUILayout.BeginFoldoutHeaderGroup(modeSettings, "Modes");
            if (modeSettings)
            {
                DisplaySpawnMode();

                DisplaySpawnArea();

                DisplaySpawnLocation();

                DisplaySpawnDirection();

                DisplayTargetEntity();

            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //Editor
            DisplayEditorSettings();

            //Other settings
            DisplayOtherSettings();

            DisplayEventSettings();

            DisplayButtons();

            serializedObject.ApplyModifiedProperties();
        }

        #region Draw Helpers
        /// <summary>
        /// Draw Circle handles
        /// </summary>
        private void DrawCircle(Vector3 center, float innerRadius, float outerRadius, int index)
        {
            GetCircleColor(index, false);
            Handles.DrawWireDisc(center, Vector3.up, outerRadius);
            GetCircleColor(index, true);
            Handles.DrawWireDisc(center, Vector3.up, innerRadius);
        }
        private void DrawRectangle(Vector3 center, Vector2 size, int index)
        {
            Vector3[] points = { center + new Vector3(size.x/2f, 0f, size.y/2f),
                                 center + new Vector3(-size.x/2f, 0f, size.y/2f),
                                 center + new Vector3(-size.x/2f, 0f, -size.y/2f),
                                 center + new Vector3(size.x/2f, 0f, -size.y/2f)};
            Handles.DrawSolidRectangleWithOutline(points, GetRectangleColor(index, true), GetRectangleColor(index, false));
        }
        private void DrawRectPrism(Vector3 center, int index)
        {
            GetPrismColor(index);
            m_BoxBounds.size = sc.GetRectPrismSize();
            m_BoxBounds.center = center;
            m_BoxBounds.DrawHandle();
        }
        private void GetCircleColor(int index, bool inner)
        {
            //Set color
            if (!cpc.checkPoints[index].GetActive())
            {
                Handles.color = GlobalSettings.GrayedOutColor;
            }
            else if (inner)
            {
                Handles.color = GlobalSettings.InnerRadiusColor;
            }
            else
            {
                Handles.color = GlobalSettings.OuterRadiusColor;
            }
        }
        private Color GetRectangleColor(int index, bool face)
        {
            //Set color
            if (!cpc.checkPoints[index].GetActive())
            {
                if (face) {
                    return GlobalSettings.GrayedOutFaceColor;
                }
                return GlobalSettings.GrayedOutColor;
            }
            else if (face)
            {
                return GlobalSettings.RectangleFaceColor;
            }
            else
            {
                return GlobalSettings.RectangleOutlineColor;
            }
        }
        private void GetPrismColor(int index)
        {
            //Set color
            if (!cpc.checkPoints[index].GetActive())
            {
                Handles.color = GlobalSettings.GrayedOutColor;
            }
            else
            {
                Handles.color = GlobalSettings.PrismColor;
            }
        }
        private void DrawMinMaxSlider(float minVal, float maxVal)
        {
            EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, 0f, 20f);
            circleRadProp.vector2Value = new Vector2(minVal, maxVal);
        }
        #endregion

        #region Inspector Helpers
        private void DisplayEditorSettings()
        {
            EditorGUI.indentLevel = 0;
            editorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(editorSettings, "Editor Settings");
            if (editorSettings)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(hideGizmosProp, hideGizmosGUI);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private void DisplayOtherSettings()
        {
            EditorGUI.indentLevel = 0;
            otherSettings = EditorGUILayout.BeginFoldoutHeaderGroup(otherSettings, "Other Settings");
            if (otherSettings)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(checkForColProp, checkForColGUI);
                EditorGUILayout.PropertyField(equalSpawnChanceProp, equalSpawnChanceGUI);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private void DisplayEventSettings()
        {
            EditorGUI.indentLevel = 0;
            eventSettings = EditorGUILayout.BeginFoldoutHeaderGroup(eventSettings, "Events");
            if (eventSettings)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.Space(1);
                EditorGUILayout.PropertyField(onSpawnProp, new GUIContent("OnSpawn", "Triggers when an entity successfully spawns."));
                EditorGUILayout.PropertyField(onExitProp, new GUIContent("OnExit", "Triggers when a stop condition is met."));
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private void DisplaySpawnArea()
        {
            //Spawn Area//
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(spawnAreaProp, spawnAreaGUI);
            EditorGUILayout.Space(0.5f);
            EditorGUI.indentLevel += 1;
            if (spawnAreaProp.enumValueIndex == (int)SpawnArea.Circle)
            {
                circleRadProp.vector2Value = EditorGUILayout.Vector2Field(circleGUI, circleRadProp.vector2Value);
                DrawMinMaxSlider(circleRadProp.vector2Value.x, circleRadProp.vector2Value.y);
            }
            else if (spawnAreaProp.enumValueIndex == (int)SpawnArea.Sphere)
            {
                EditorGUILayout.PropertyField(sphereRadiusProp, sphereRadiusGUI);
            }
            else if (spawnAreaProp.enumValueIndex == (int)SpawnArea.Rect)
            {
                EditorGUILayout.PropertyField(rectSizeProp, rectSizeGUI);
            }
            else if (spawnAreaProp.enumValueIndex == (int)SpawnArea.RectPrism)
            {
                //sc.rectPrismSize = EditorGUILayout.Vector3Field(rectPrismGUI, sc.rectPrismSize);
                EditorGUILayout.PropertyField(rectPrismSizeProp, rectPrismGUI);
            }
            EditorGUILayout.PropertyField(offsetProp, offsetGUI);
            EditorGUILayout.Space(0.5f);
        }
        private void DisplaySpawnMode()
        {
            //Spawn Mode//
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(spawnModeProp, spawnModeGUI);
            EditorGUILayout.Space(0.5f);

            EditorGUI.indentLevel += 1;
            EditorGUIUtility.labelWidth += GlobalSettings.indentField;
            //Constant Spawn Rate
            if (spawnModeProp.enumValueIndex == (int)SpawnMode.Constant)
            {
                EditorGUILayout.PropertyField(spawnDelayProp, spawnDelayGUI);
                EditorGUILayout.Space(1f);
                //Exit Condition
                DisplayExitCondition();
                //Max spawn count
                EditorGUILayout.PropertyField(enableSpawnMaxProp, enableMaxGUI);
                if (enableSpawnMaxProp.boolValue)
                {
                    DisplayIndentedProperty(constMaxProp, spawnMaxGUI);
                }
                EditorGUILayout.Space(1f);
            }
            else if (spawnModeProp.enumValueIndex == (int)SpawnMode.SingleBurst)
            {
                EditorGUILayout.PropertyField(atsProp, atsGUI);
                EditorGUILayout.Space(1f);
            }
            else if (spawnModeProp.enumValueIndex == (int)SpawnMode.Refill)
            {
                EditorGUILayout.PropertyField(refillDelayProp, refillDelayGUI);
                EditorGUILayout.Space(1f);
                //Exit condition
                DisplayExitCondition();
                EditorGUILayout.PropertyField(enableRefillMaxProp, enableMaxGUI);
                if (enableRefillMaxProp.boolValue)
                {
                    DisplayIndentedProperty(refillMaxProp, spawnMaxGUI);
                }
                EditorGUILayout.Space(1f);
            }
            EditorGUIUtility.labelWidth -= GlobalSettings.indentField;
        }
        private void DisplaySpawnLocation()
        {
            //Spawn Location//
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(spawnLocationProp, spawnLocationGUI);
            EditorGUILayout.Space(0.5f);
        }
        private void DisplaySpawnDirection()
        {
            //Spawn Direction//
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(spawnDirectionProp, spawnDirectionGUI);
            EditorGUILayout.Space(0.5f);
            if(spawnDirectionProp.enumValueIndex == (int)SpawnDirection.Custom)
            {
                DisplayIndentedProperty(customDirectionProp, customDirectionGUI);
            }
            else if(spawnDirectionProp.enumValueIndex == (int)SpawnDirection.FaceEntity)
            {
                DisplayIndentedProperty(includeYPlaneProp, includeYPlaneGUI);
            }
        }
        private void DisplayIndentedProperty(SerializedProperty property, GUIContent content)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(property, content);
            EditorGUI.indentLevel -= 1;
        }
        private void DisplayExitCondition()
        {
            EditorGUILayout.PropertyField(exitConditionProp, exitConditionGUI);
            EditorGUI.indentLevel += 1;
            if (exitConditionProp.enumValueIndex == (int)ExitCondition.SpawnCount)
            {
                EditorGUILayout.PropertyField(exitSpawnCountProp, exitSpawnCountGUI);
            }
            else if (exitConditionProp.enumValueIndex == (int)ExitCondition.Time)
            {
                EditorGUILayout.PropertyField(exitTimeProp, exitTimeGUI);
            }
            EditorGUI.indentLevel -= 1;
        }
        private void DisplayButtons()
        {
            if (!Application.isPlaying)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
            EditorGUILayout.Space(7f);
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Restart Spawner"))
            {
                sc.ResetSpawner();
            }
            if (GUILayout.Button("Destroy All Entities"))
            {
                sc.DestroySpawnedEntities();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5f);
            GUI.enabled = true;
        }
        private void DisplayTargetEntity()
        {
            EditorGUI.indentLevel = 1;
            if (spawnDirectionProp.enumValueIndex == (int)SpawnDirection.FaceEntity || spawnLocationProp.enumValueIndex == (int)SpawnLocation.NearestToEntity)
            {
                EditorGUILayout.PropertyField(targetEntityProp, targetEntityGUI);
                if (targetEntityProp.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("Target entity not assigned.", MessageType.Error);
                }
            }
        }
        #endregion
    }
}
