using System;
using Lean.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lean.Gui
{
	/// <summary>
	///     This component adds a safe area to your UI. This is mainly used to prevent UI elements from going through notches
	///     on mobile devices.
	///     This component should be added to a GameObject that is a child of your Canvas root.
	/// </summary>
	[ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanSafeArea")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Safe Area")]
    public class LeanSafeArea : UIBehaviour
    {
        [SerializeField] private bool horizontal = true;
        [Range(0.0f, 1.0f)] [SerializeField] private Vector2 horizontalRange = new Vector2(0.0f, 1.0f);
        [SerializeField] private bool vertical = true;
        [Range(0.0f, 1.0f)] [SerializeField] private Vector2 verticalRange = new Vector2(0.0f, 1.0f);

        [NonSerialized] private RectTransform cachedRectTransform;

        [NonSerialized] private bool cachedRectTransformSet;

        /// <summary>Should you be able to drag horizontally?</summary>
        public bool Horizontal
        {
            set => horizontal = value;
            get => horizontal;
        }

        public Vector2 HorizontalRange
        {
            set => horizontalRange = value;
            get => horizontalRange;
        }

        /// <summary>Should you be able to drag vertically?</summary>
        public bool Vertical
        {
            set => vertical = value;
            get => vertical;
        }

        public Vector2 VerticalRange
        {
            set => verticalRange = value;
            get => verticalRange;
        }

        protected virtual void Update()
        {
            UpdateSafeArea();
        }

        /// <summary>This method will instantly update the safe area RectTransform.</summary>
        [ContextMenu("Update Safe Area")]
        public void UpdateSafeArea()
        {
            if (cachedRectTransformSet == false)
            {
                cachedRectTransform = GetComponent<RectTransform>();
                cachedRectTransformSet = true;
            }

            Rect safeRect = Screen.safeArea;
            int screenW = Screen.width;
            int screenH = Screen.height;
            Vector2 safeMin = safeRect.min;
            Vector2 safeMax = safeRect.max;

            if (horizontal == false)
            {
                safeMin.x = 0.0f;
                safeMax.x = screenW;
            }
            else
            {
                safeMin.x = Mathf.Max(safeMin.x, horizontalRange.x * screenW);
                safeMax.x = Mathf.Min(safeMax.x, horizontalRange.y * screenW);
            }

            if (vertical == false)
            {
                safeMin.y = 0.0f;
                safeMax.y = screenH;
            }
            else
            {
                safeMin.y = Mathf.Max(safeMin.y, verticalRange.x * screenH);
                safeMax.y = Mathf.Min(safeMax.y, verticalRange.y * screenH);
            }

            cachedRectTransform.anchorMin = new Vector2(safeMin.x / screenW, safeMin.y / screenH);
            cachedRectTransform.anchorMax = new Vector2(safeMax.x / screenW, safeMax.y / screenH);
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanSafeArea;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanSafeArea_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("horizontal", "Should you be able to drag horizontally?");
            if (Any(tgts, t => t.Horizontal))
            {
                BeginIndent();
                DrawMinMax("horizontalRange", 0.0f, 1.0f, "", "Range");
                EndIndent();
            }
            Draw("vertical", "Should you be able to drag vertically?");
            if (Any(tgts, t => t.Vertical))
            {
                BeginIndent();
                DrawMinMax("verticalRange", 0.0f, 1.0f, "", "Range");
                EndIndent();
            }
        }
    }
}

#endif