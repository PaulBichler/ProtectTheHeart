﻿using System;
using UnityEditor;
using UnityEngine;

namespace Lean.Transition
{
    /// <summary>This enum allows you to pick the ease type used by most transition methods.</summary>
    public enum LeanEase
    {
        // Basic
        Linear,
        Smooth = 100,
        Accelerate = 200,
        Decelerate = 250,
        Elastic = 300,
        Back = 400,
        Bounce = 500,

        // Advanced
        SineIn = 1000,
        SineOut,
        SineInOut,

        QuadIn = 1100,
        QuadOut,
        QuadInOut,

        CubicIn = 1200,
        CubicOut,
        CubicInOut,

        QuartIn = 1300,
        QuartOut,
        QuartInOut,

        QuintIn = 1400,
        QuintOut,
        QuintInOut,

        ExpoIn = 1500,
        ExpoOut,
        ExpoInOut,

        CircIn = 1600,
        CircOut,
        CircInOut,

        BackIn = 1700,
        BackOut,
        BackInOut,

        ElasticIn = 1800,
        ElasticOut,
        ElasticInOut,

        BounceIn = 1900,
        BounceOut,
        BounceInOut
    }
}

#if UNITY_EDITOR

namespace Lean.Transition.Editor
{
    [CustomPropertyDrawer(typeof(LeanEase))]
    public class LeanEase_Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (GUI.Button(position, ((LeanEase) property.intValue).ToString(), EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();
                string[] names = Enum.GetNames(typeof(LeanEase));
                Array values = Enum.GetValues(typeof(LeanEase));

                for (int i = 0; i < values.Length; i++)
                {
                    string name = names[i];
                    int value = (int) values.GetValue(i);

                    if (value >= 1000)
                        name = "Advanced/" + name;

                    menu.AddItem(new GUIContent(name), value == property.intValue, () =>
                    {
                        property.intValue = value;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }

                menu.ShowAsContext();
            }

            EditorGUI.EndProperty();
        }

        private class Entry
        {
            public GUIContent Content;

            public int Value;

            public Entry()
            {
            }

            public Entry(string newName, LeanEase newValue)
            {
                Content = new GUIContent(newName);
                Value = (int) newValue;
            }
        }
    }
}

#endif