using System;
using Lean.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lean.Gui
{
	/// <summary>
	///     This component allows you to change the size of the Target RectTransform based on the size of this one.
	///     This is very useful for text that needs to be inside a parent container, but you don't know how big that container
	///     should be.
	/// </summary>
	[ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanSizer")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Sizer")]
    public class LeanSizer : UIBehaviour
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private bool scale;
        [SerializeField] private bool horizontal;
        [SerializeField] private float horizontalPadding;
        [SerializeField] private bool vertical;
        [SerializeField] private float verticalPadding;

        [NonSerialized] private RectTransform cachedRectTransform;

        [NonSerialized] private bool cachedRectTransformSet;

        /// <summary>The RectTransform whose size we want to modify.</summary>
        public RectTransform Target
        {
            set
            {
                if (target != value)
                {
                    target = value;
                    UpdateSize();
                }
            }
            get => target;
        }

        /// <summary>Factor in scale when reading the size of the target?</summary>
        public bool Scale
        {
            set
            {
                if (scale != value)
                {
                    scale = value;
                    UpdateSize();
                }
            }
            get => scale;
        }

        /// <summary>Match the scale horizontally?</summary>
        public bool Horizontal
        {
            set
            {
                if (horizontal != value)
                {
                    horizontal = value;
                    UpdateSize();
                }
            }
            get => horizontal;
        }

        public float HorizontalPadding
        {
            set
            {
                if (horizontalPadding != value)
                {
                    horizontalPadding = value;
                    UpdateSize();
                }
            }
            get => horizontalPadding;
        }

        /// <summary>Match the scale vertically?</summary>
        public bool Vertical
        {
            set
            {
                if (vertical != value)
                {
                    vertical = value;
                    UpdateSize();
                }
            }
            get => vertical;
        }

        public float VerticalPadding
        {
            set
            {
                if (verticalPadding != value)
                {
                    verticalPadding = value;
                    UpdateSize();
                }
            }
            get => verticalPadding;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateSize();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            UpdateSize();
        }
#endif

        [ContextMenu("Update Size")]
        public void UpdateSize()
        {
            if (Target != null)
            {
                if (cachedRectTransformSet == false)
                {
                    cachedRectTransform = GetComponent<RectTransform>();
                    cachedRectTransformSet = true;
                }

                Vector2 targetSize = Target.sizeDelta;
                Vector3 scale = cachedRectTransform.localScale;
                Vector2 sizeDelta = cachedRectTransform.sizeDelta;

                if (Horizontal)
                {
                    targetSize.x = sizeDelta.x + HorizontalPadding;

                    if (Scale)
                        targetSize.x *= scale.x;
                }

                if (Vertical)
                {
                    targetSize.y = sizeDelta.y + VerticalPadding;

                    if (Scale)
                        targetSize.y *= scale.y;
                }

                Target.sizeDelta = targetSize;
            }
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanSizer;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanSizer_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            BeginError(Any(tgts, t => t.Target == null));
            Draw("target", "The RectTransform whose size we want to modify.");
            EndError();
            Draw("scale", "Factor in scale when reading the size of the target?");

            Separator();

            Draw("horizontal", "Match the scale horizontally?");

            if (Any(tgts, t => t.Horizontal))
            {
                BeginIndent();
                Draw("horizontalPadding", "", "Padding");
                EndIndent();
            }

            Separator();

            Draw("vertical", "Match the scale vertically?");

            if (Any(tgts, t => t.Vertical))
            {
                BeginIndent();
                Draw("verticalPadding", "", "Padding");
                EndIndent();
            }
        }
    }
}

#endif