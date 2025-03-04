using UnityEditor;
using UnityEngine;
using SmartPoint.Utility;


namespace SmartPoint.Editors
{
    /// <summary>
    /// Custom Property Drawer. This is used to customize how entities are displayed in the inspector
    /// </summary>
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Entity))]
    public class EntityDrawer : PropertyDrawer
    {
        SerializedProperty entity;
        SerializedProperty spawnChance;
        SerializedProperty equalSpawnChance;
        SpawnController sc;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            EditorGUI.BeginProperty(position, label, property);
            EditorGUIUtility.labelWidth = 128;

            entity = property.FindPropertyRelative("entity");
            spawnChance = property.FindPropertyRelative("spawnChance");
            equalSpawnChance = property.serializedObject.FindProperty("equalSpawnChance");
            sc = property.serializedObject.targetObject as SpawnController;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), entity);

            if (equalSpawnChance.boolValue)
            {
                GUI.enabled = false;
                spawnChance.floatValue = MathUtility.EqualSpawnChance(sc.GetSpawnChances().Length, sc.GetSpawnChances(), spawnChance.floatValue);
            }
            else
            {
                //Clamp spawn chance to sum to 100
                spawnChance.floatValue = MathUtility.CalculateMaxSpawnChance(spawnChance.floatValue, sc.GetSpawnChances());
            }
            EditorGUI.PrefixLabel(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 1.3f, position.width, EditorGUIUtility.singleLineHeight), new GUIContent("Spawn Chance", "The probability that this entity will spawn."));
            spawnChance.floatValue = EditorGUI.Slider(new Rect(position.x+100f, position.y+ EditorGUIUtility.singleLineHeight*1.3f, position.width-100, EditorGUIUtility.singleLineHeight), Mathf.Round(spawnChance.floatValue*100f) / 100f, 0, 100);
            property.serializedObject.ApplyModifiedProperties();
            
            GUI.enabled = true;

        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalLine = 2.5f;

            if (property.isExpanded)
            {
                totalLine += 2;
            }

            return EditorGUIUtility.singleLineHeight * totalLine + EditorGUIUtility.standardVerticalSpacing * (totalLine - 1);
        }

    }

    
#endif
}
