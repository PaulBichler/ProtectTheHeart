using UnityEditor;
using UnityEngine;

namespace Lean.Transition.Editor
{
    /// <summary>This allows you to define a transition template.</summary>
    [CreateAssetMenu(fileName = "NewTemplate", menuName = "Lean/Transition/Template")]
    public class LeanTemplate : ScriptableObject
    {
        [Multiline(100)] public string Body;
    }

    [CustomEditor(typeof(LeanTemplate))]
    public class LeanTemplate_Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 50.0f;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Body"));
            EditorGUIUtility.labelWidth = labelWidth;

            serializedObject.ApplyModifiedProperties();
        }
    }
}