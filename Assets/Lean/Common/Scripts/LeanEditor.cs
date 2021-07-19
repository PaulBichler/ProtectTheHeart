﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lean.Common
{
    /// <summary>This is the base class for all inspectors.</summary>
    public abstract class LeanEditor : UnityEditor.Editor
    {
        private static SerializedObject data;

        private static readonly GUIContent customContent = new GUIContent();

        private static readonly List<Color> colors = new List<Color>();

        private static readonly List<float> labelWidths = new List<float>();

        private static readonly List<bool> mixedValues = new List<bool>();

        public virtual void OnSceneGUI()
        {
            OnScene();

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        public void GetTargets<T>(out T tgt, out T[] tgts)
            where T : Object
        {
            tgt = (T) target;
            tgts = Array.ConvertAll(targets, item => (T) item);
        }

        public static void SetData(SerializedObject newData)
        {
            data = newData;
        }

        public override void OnInspectorGUI()
        {
            SetData(serializedObject);

            ClearStacks();

            Separator();

            OnInspector();

            Separator();

            serializedObject.ApplyModifiedProperties();

            data = null;
        }

        protected void Each<T>(T[] tgts, Action<T> update, bool dirty = false)
            where T : Object
        {
            foreach (T t in tgts)
            {
                update(t);

                if (dirty)
                    EditorUtility.SetDirty(t);
            }
        }

        protected bool Any<T>(T[] tgts, Func<T, bool> check)
            where T : Object
        {
            foreach (T t in tgts)
                if (check(t))
                    return true;

            return false;
        }

        protected bool All<T>(T[] tgts, Func<T, bool> check)
            where T : Object
        {
            foreach (T t in tgts)
                if (check(t) == false)
                    return false;

            return true;
        }

        public static Rect Reserve(float height = 19.0f)
        {
            Rect rect =
                EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(string.Empty, GUILayout.Height(height), GUILayout.ExpandWidth(true),
                GUILayout.MinWidth(0.0f));
            EditorGUILayout.EndVertical();

            return rect;
        }

        public static void Info(string message)
        {
            EditorGUILayout.HelpBox(StripRichText(message), MessageType.Info);
                // Help boxes can't display rich text for some reason, so strip it
        }

        public static void Warning(string message)
        {
            EditorGUILayout.HelpBox(StripRichText(message), MessageType.Warning);
                // Help boxes can't display rich text for some reason, so strip it
        }

        public static void Error(string message)
        {
            EditorGUILayout.HelpBox(StripRichText(message), MessageType.Error);
                // Help boxes can't display rich text for some reason, so strip it
        }

        public static void Separator()
        {
            EditorGUILayout.Separator();
        }

        public static void BeginIndent()
        {
            EditorGUI.indentLevel += 1;
        }

        public static void EndIndent()
        {
            EditorGUI.indentLevel -= 1;
        }

        public static bool Button(string text)
        {
            return GUILayout.Button(text);
        }

        public static bool HelpButton(string helpText, MessageType type, string buttonText, float buttonWidth)
        {
            bool clicked = false;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.HelpBox(helpText, type);

                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.wordWrap = true;

                clicked = GUILayout.Button(buttonText, style, GUILayout.ExpandHeight(true),
                    GUILayout.Width(buttonWidth));
            }
            EditorGUILayout.EndHorizontal();

            return clicked;
        }

        public static void ClearStacks()
        {
            while (colors.Count > 0)
                EndColor();

            while (labelWidths.Count > 0)
                EndLabelWidth();

            while (mixedValues.Count > 0)
                EndMixedValue();
        }

        public static void BeginMixedValue(bool mixed = true)
        {
            mixedValues.Add(EditorGUI.showMixedValue);

            EditorGUI.showMixedValue = mixed;
        }

        public static void EndMixedValue()
        {
            if (mixedValues.Count > 0)
            {
                int index = mixedValues.Count - 1;

                EditorGUI.showMixedValue = mixedValues[index];

                mixedValues.RemoveAt(index);
            }
        }

        public static void BeginDisabled(bool disabled = true)
        {
            EditorGUI.BeginDisabledGroup(disabled);
        }

        public static void EndDisabled()
        {
            EditorGUI.EndDisabledGroup();
        }

        public static void BeginError(bool error = true)
        {
            BeginColor(Color.red, error);
        }

        public static void EndError()
        {
            EndColor();
        }

        public static void BeginColor(Color color, bool show = true)
        {
            colors.Add(GUI.color);

            GUI.color = show ? color : colors[0];
        }

        public static void EndColor()
        {
            if (colors.Count > 0)
            {
                int index = colors.Count - 1;

                GUI.color = colors[index];

                colors.RemoveAt(index);
            }
        }

        public static void BeginLabelWidth(float width)
        {
            labelWidths.Add(EditorGUIUtility.labelWidth);

            EditorGUIUtility.labelWidth = width;
        }

        public static void EndLabelWidth()
        {
            if (labelWidths.Count > 0)
            {
                int index = labelWidths.Count - 1;

                EditorGUIUtility.labelWidth = labelWidths[index];

                labelWidths.RemoveAt(index);
            }
        }

        public static bool DrawFoldout(string overrideText, string overrideTooltip, string propertyPath = "m_Name")
        {
            SerializedProperty property = GetPropertyAndSetCustomContent(propertyPath, overrideTooltip, overrideText);

            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, customContent);

            return property.isExpanded;
        }

        public static bool DrawExpand(string propertyPath, ref bool modified, string overrideTooltip = null,
            string overrideText = null)
        {
            SerializedProperty property = GetPropertyAndSetCustomContent(propertyPath, overrideTooltip, overrideText);
            Rect rect = EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(string.Empty, GUILayout.Height(EditorGUI.GetPropertyHeight(property)));
            EditorGUILayout.EndVertical();
            Rect rectF = rect;
            rectF.height = 16;

            property.isExpanded = EditorGUI.Foldout(rectF, property.isExpanded, GUIContent.none);

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(rect, property, customContent, true);

            modified = EditorGUI.EndChangeCheck();

            return property.isExpanded;
        }

        public static bool DrawExpand(string propertyPath, string overrideTooltip = null, string overrideText = null)
        {
            bool modified = false;
            return DrawExpand(propertyPath, ref modified, overrideTooltip, overrideText);
        }

        public static bool Draw(string propertyPath, string overrideTooltip = null, string overrideText = null)
        {
            SerializedProperty property = GetPropertyAndSetCustomContent(propertyPath, overrideTooltip, overrideText);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(property, customContent, true);

            return EditorGUI.EndChangeCheck();
        }

        public static void Draw(string propertyPath, ref bool dirty, string overrideTooltip = null,
            string overrideText = null)
        {
            if (Draw(propertyPath, overrideTooltip, overrideText))
                dirty = true;
        }

        public static void Draw(string propertyPath, ref bool dirty1, ref bool dirty2, string overrideTooltip = null,
            string overrideText = null)
        {
            if (Draw(propertyPath, overrideTooltip, overrideText))
            {
                dirty1 = true;
                dirty2 = true;
            }
        }

        public static bool DrawMinMax(string propertyPath, float min, float max, string overrideTooltip = null,
            string overrideText = null)
        {
            SerializedProperty property = GetPropertyAndSetCustomContent(propertyPath, overrideTooltip, overrideText);
            Vector2 value = property.vector2Value;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.MinMaxSlider(customContent, ref value.x, ref value.y, min, max);

            if (EditorGUI.EndChangeCheck())
            {
                property.vector2Value = value;

                return true;
            }

            return false;
        }

        public static bool DrawIntPopup(int[] values, GUIContent[] contents, string propertyPath,
            string overrideTooltip = null, string overrideText = null)
        {
            SerializedProperty property = GetPropertyAndSetCustomContent(propertyPath, overrideTooltip, overrideText);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.IntPopup(property, contents, values, customContent);

            return EditorGUI.EndChangeCheck();
        }

        public static void DrawIntPopup(int[] values, GUIContent[] contents, string propertyPath, ref bool modified,
            string overrideTooltip = null, string overrideText = null)
        {
            if (DrawIntPopup(values, contents, propertyPath, overrideTooltip, overrideText))
                modified = true;
        }

        public static bool DrawLayer(string propertyPath, string overrideTooltip = null, string overrideText = null)
        {
            SerializedProperty property = GetPropertyAndSetCustomContent(propertyPath, overrideTooltip, overrideText);
            int value = property.intValue;

            EditorGUI.BeginChangeCheck();

            value = EditorGUILayout.LayerField(customContent, value);

            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = value;

                return true;
            }

            return false;
        }

        public static bool DrawEulerAngles(string propertyPath, string overrideTooltip = null,
            string overrideText = null)
        {
            SerializedProperty property = GetPropertyAndSetCustomContent(propertyPath, overrideTooltip, overrideText);
            Vector3 value = property.quaternionValue.eulerAngles;

            EditorGUI.BeginChangeCheck();

            BeginMixedValue(property.hasMultipleDifferentValues);
            value = EditorGUILayout.Vector3Field(customContent, value);
            EndMixedValue();

            if (EditorGUI.EndChangeCheck())
            {
                property.quaternionValue = Quaternion.Euler(value);

                return true;
            }

            return false;
        }

        protected void DirtyAndUpdate()
        {
            for (int i = targets.Length - 1; i >= 0; i--)
                EditorUtility.SetDirty(targets[i]);

            serializedObject.Update();
        }

        private static SerializedProperty GetPropertyAndSetCustomContent(string propertyPath, string overrideTooltip,
            string overrideText)
        {
            SerializedProperty property = data.FindProperty(propertyPath);

            customContent.text = string.IsNullOrEmpty(overrideText) == false ? overrideText : property.displayName;
            customContent.tooltip = string.IsNullOrEmpty(overrideTooltip) == false ? overrideTooltip : property.tooltip;
            customContent.tooltip = StripRichText(customContent.tooltip);
                // Tooltips can't display rich text for some reason, so strip it

            return property;
        }

        private static string StripRichText(string s)
        {
            return s.Replace("<b>", "").Replace("</b>", "");
        }

        protected virtual void OnInspector()
        {
        }

        protected virtual void OnScene()
        {
        }
    }
}

#endif