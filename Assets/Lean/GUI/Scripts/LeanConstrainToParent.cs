using System;
using Lean.Common;
using UnityEditor;
using UnityEngine;

namespace Lean.Gui
{
    /// <summary>This component will automatically constrain the current <b>RectTransform</b> to its parent.</summary>
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanConstrainToParent")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Constrain To Parent")]
    public class LeanConstrainToParent : MonoBehaviour
    {
        [SerializeField] private bool horizontal = true;
        [SerializeField] private bool vertical = true;

        [NonSerialized] private RectTransform cachedParentRectTransform;

        [NonSerialized] private RectTransform cachedRectTransform;

        /// <summary>Constrain horizontally?</summary>
        public bool Horizontal
        {
            set => horizontal = value;
            get => horizontal;
        }

        /// <summary>Constrain vertically?</summary>
        public bool Vertical
        {
            set => vertical = value;
            get => vertical;
        }

        protected virtual void LateUpdate()
        {
            if (cachedParentRectTransform != cachedRectTransform.parent)
                cachedParentRectTransform = cachedRectTransform.parent as RectTransform;

            if (cachedParentRectTransform != null)
            {
                Vector2 anchoredPosition = cachedRectTransform.anchoredPosition;
                Rect rect = cachedRectTransform.rect;
                Rect boundary = cachedParentRectTransform.rect;

                if (horizontal)
                {
                    boundary.xMin -= rect.xMin;
                    boundary.xMax -= rect.xMax;

                    anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, boundary.xMin, boundary.xMax);
                }

                if (vertical)
                {
                    boundary.yMin -= rect.yMin;
                    boundary.yMax -= rect.yMax;

                    anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, boundary.yMin, boundary.yMax);
                }

                cachedRectTransform.anchoredPosition = anchoredPosition;
            }
        }

        protected virtual void OnEnable()
        {
            cachedRectTransform = GetComponent<RectTransform>();
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanConstrainToParent;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanConstrainToParent_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("horizontal", "Constrain horizontally?");
            Draw("vertical", "Constrain vertically?");
        }
    }
}

#endif