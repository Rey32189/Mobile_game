using UnityEngine;
using UnityEditor;

namespace SmartPoint.Editors
{
    /// <summary>
    /// Custom Property Drawer. This is used to customize how checkpoints are displayed in the inspector
    /// </summary>
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CheckPoint))]
    public class CheckPointDrawer : PropertyDrawer
    {
        SerializedProperty activeProperty;
        SerializedProperty posProperty;
        SerializedProperty boundsProperty;
        SerializedProperty dirProperty;
        SerializedProperty absPosProp;
        SerializedProperty addTriggerProp;
        SerializedProperty activeModeProp;
        CheckPointController cpc;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            EditorGUI.BeginProperty(position, label, property);
            EditorGUIUtility.labelWidth = 128;

            activeProperty = property.FindPropertyRelative("isActive");
            posProperty = property.FindPropertyRelative("position");
            boundsProperty = property.FindPropertyRelative("bounds");
            dirProperty = property.FindPropertyRelative("direction");
            absPosProp = property.serializedObject.FindProperty("ShowAbsolutePosition");
            addTriggerProp = property.serializedObject.FindProperty("AddTrigger");
            activeModeProp = property.serializedObject.FindProperty("activationMode");
            cpc = property.serializedObject.targetObject as CheckPointController;

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);
            if (property.isExpanded)
            {
                EditorGUI.PropertyField(new Rect(position.x + 20, position.y + EditorGUIUtility.singleLineHeight, position.width - 20, EditorGUIUtility.singleLineHeight), activeProperty, new GUIContent("isActive", "If true, this checkpoint starts active.\n\nMarking a checkpoint as active is simply a way to denote which checkpoints have already been visited."));
                if (!absPosProp.boolValue)
                    EditorGUI.PropertyField(new Rect(position.x + 20, position.y + EditorGUIUtility.singleLineHeight * 2 + 2, position.width - 20, EditorGUIUtility.singleLineHeight), posProperty, new GUIContent("Position", "This checkpoint's position, displayed in relative space. See 'Editor' settings to enable absolute positions."));
                else
                    posProperty.vector3Value = EditorGUI.Vector3Field(new Rect(position.x + 20, position.y + EditorGUIUtility.singleLineHeight * 2 + 2, position.width - 20, EditorGUIUtility.singleLineHeight), new GUIContent("Position", "The checkpoint's position."), posProperty.vector3Value + cpc.transform.position) - cpc.transform.position;
                EditorGUI.PropertyField(new Rect(position.x + 20, position.y + EditorGUIUtility.singleLineHeight * 3 + 4, position.width - 20, EditorGUIUtility.singleLineHeight), dirProperty, new GUIContent("Direction", "The checkpoint's direction. Only used if Align to Direction is checked."));
                //Bounds, contingent on collider being active
                if (addTriggerProp.boolValue || activeModeProp.enumValueIndex == (int)ActivationMode.ActivateOnCollision)
                {
                    EditorGUI.PropertyField(new Rect(position.x + 20, position.y + EditorGUIUtility.singleLineHeight * 4 + 6, position.width - 20, EditorGUIUtility.singleLineHeight * 3), boundsProperty, new GUIContent("Bounds", "Bounds for the collision box. Only used in Activation Mode 'activate on collision'"));
                }
            }

            property.serializedObject.ApplyModifiedProperties();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            addTriggerProp = property.serializedObject.FindProperty("AddTrigger");
            activeModeProp = property.serializedObject.FindProperty("activationMode");
            int totalLine = 1;

            if (property.isExpanded)
            {
                totalLine += 6;
                if(!addTriggerProp.boolValue && activeModeProp.enumValueIndex != (int)ActivationMode.ActivateOnCollision)
                {
                    totalLine -= 3;
                }
            }

            return EditorGUIUtility.singleLineHeight * totalLine + EditorGUIUtility.standardVerticalSpacing * (totalLine - 1);
        }
    }
}
#endif