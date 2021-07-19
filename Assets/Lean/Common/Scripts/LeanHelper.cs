using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lean.Common
{
    /// <summary>This class contains useful methods used in almost all <b>LeanTouch</b> code.</summary>
    public static class LeanHelper
    {
        public const string HelpUrlPrefix = "https://carloswilkes.github.io/Documentation/LeanCommon#";

        public const string PlusHelpUrlPrefix = "https://carloswilkes.github.io/Documentation/LeanCommonPlus#";

        public const string ComponentPathPrefix = "Lean/Common/Lean ";

#if UNITY_EDITOR
        /// <summary>This method creates an empty GameObject prefab at the current asset folder</summary>
        public static GameObject CreateAsset(string name)
        {
            GameObject gameObject = new GameObject(name);
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
                path = "Assets";

            path = AssetDatabase.GenerateUniqueAssetPath(path + "/" + name + ".prefab");
#if UNITY_2018_3_OR_NEWER
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, path);
#else
			var prefab = PrefabUtility.CreatePrefab(path, gameObject);
#endif
            Object.DestroyImmediate(gameObject);

            Selection.activeObject = prefab;

            return prefab;
        }
#endif

	    /// <summary>
	    ///     This method allows you to create a UI element with the specified component and specified parent, with behavior
	    ///     consistent with Unity's built-in UI element creation.
	    /// </summary>
	    public static T CreateElement<T>(Transform parent)
            where T : Component
        {
            GameObject gameObject = new GameObject(typeof(T).Name);

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + typeof(T).Name);
#endif

            T component = gameObject.AddComponent<T>();

            // Auto attach to canvas?
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                Canvas canvas = Object.FindObjectOfType<Canvas>();

                if (canvas == null)
                {
                    canvas =
                        new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                            typeof(GraphicRaycaster)).GetComponent<Canvas>();

                    canvas.gameObject.layer = LayerMask.NameToLayer("UI");

                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    // Make event system?
                    if (EventSystem.current == null)
                    {
#if ENABLE_INPUT_SYSTEM
                        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
#else
						new GameObject("EventSystem", typeof(EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
#endif
                    }
                }

                parent = canvas.transform;
            }

            gameObject.layer = parent.gameObject.layer;

            component.transform.SetParent(parent, false);

            return component;
        }

	    /// <summary>
	    ///     This method gives you the time-independent 't' value for lerp when used for dampening. This returns 1 in edit
	    ///     mode, or if dampening is less than 0.
	    /// </summary>
	    public static float GetDampenFactor(float damping, float elapsed)
        {
            if (damping < 0.0f)
                return 1.0f;

#if UNITY_EDITOR
            if (Application.isPlaying == false)
                return 1.0f;
#endif

            return 1.0f - Mathf.Exp(-damping * elapsed);
        }

        /// <summary>This method allows you to destroy the target object in game and in edit mode, and it returns null.</summary>
        public static T Destroy<T>(T o)
            where T : Object
        {
            if (o != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    Object.Destroy(o);
                else
                    Object.DestroyImmediate(o);
#else
				Object.Destroy(o);
#endif
            }

            return null;
        }

        // If currentCamera is null, this will return the camera attached to gameObject, or return Camera.main
        public static Camera GetCamera(Camera currentCamera, GameObject gameObject = null)
        {
            if (currentCamera == null)
            {
                if (gameObject != null)
                    currentCamera = gameObject.GetComponent<Camera>();

                if (currentCamera == null)
                    currentCamera = Camera.main;
            }

            return currentCamera;
        }

#if UNITY_EDITOR
        /// <summary>This method gives you the actual object behind a SerializedProperty given to you by a property drawer.</summary>
        public static T GetObjectFromSerializedProperty<T>(object target, SerializedProperty property)
        {
            string[] tokens = property.propertyPath.Replace(".Array.data[", ".[").Split('.');

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                Type type = target.GetType();

                if (target is IList)
                {
                    IList list = (IList) target;
                    int index = int.Parse(token.Substring(1, token.Length - 2));

                    target = list[index];
                }
                else
                {
                    while (type != null)
                    {
                        FieldInfo field = type.GetField(token,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        if (field != null)
                        {
                            target = field.GetValue(target);

                            break;
                        }

                        type = type.BaseType;
                    }
                }
            }

            return (T) target;
        }
#endif

        public static Vector2 Hermite(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
        {
            float mu2 = t * t;
            float mu3 = mu2 * t;
            float x = HermiteInterpolate(a.x, b.x, c.x, d.x, t, mu2, mu3);
            float y = HermiteInterpolate(a.y, b.y, c.y, d.y, t, mu2, mu3);

            return new Vector2(x, y);
        }

        private static float HermiteInterpolate(float y0, float y1, float y2, float y3, float mu, float mu2, float mu3)
        {
            float m0 = (y1 - y0) * 0.5f + (y2 - y1) * 0.5f;
            float m1 = (y2 - y1) * 0.5f + (y3 - y2) * 0.5f;
            float a0 = 2.0f * mu3 - 3.0f * mu2 + 1.0f;
            float a1 = mu3 - 2.0f * mu2 + mu;
            float a2 = mu3 - mu2;
            float a3 = -2.0f * mu3 + 3.0f * mu2;

            return a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2;
        }
    }
}