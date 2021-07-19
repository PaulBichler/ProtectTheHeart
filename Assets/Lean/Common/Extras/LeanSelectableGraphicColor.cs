using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>
	///     This component allows you to change the color of the Graphic (e.g. Image) attached to the current GameObject
	///     when selected.
	/// </summary>
	[ExecuteInEditMode]
    [RequireComponent(typeof(Graphic))]
    [HelpURL(LeanHelper.HelpUrlPrefix + "LeanSelectableGraphicColor")]
    [AddComponentMenu(LeanHelper.ComponentPathPrefix + "Selectable Graphic Color")]
    public class LeanSelectableGraphicColor : LeanSelectableBehaviour
    {
        [FSA("DefaultColor")] [SerializeField] private Color defaultColor = Color.white;
        [FSA("SelectedColor")] [SerializeField] private Color selectedColor = Color.green;

        [NonSerialized] private Graphic cachedGraphic;

        /// <summary>The default color given to the SpriteRenderer.</summary>
        public Color DefaultColor
        {
            set
            {
                defaultColor = value;
                UpdateColor();
            }
            get => defaultColor;
        }

        /// <summary>The color given to the SpriteRenderer when selected.</summary>
        public Color SelectedColor
        {
            set
            {
                selectedColor = value;
                UpdateColor();
            }
            get => selectedColor;
        }

        protected override void OnSelected()
        {
            UpdateColor();
        }

        protected override void OnDeselected()
        {
            UpdateColor();
        }

        public void UpdateColor()
        {
            if (cachedGraphic == null) cachedGraphic = GetComponent<Graphic>();

            Color color = Selectable != null && Selectable.IsSelected ? selectedColor : defaultColor;

            cachedGraphic.color = color;
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Common.Editor
{
    using TARGET = LeanSelectableGraphicColor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanSelectableGraphicColor_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            bool updateColor = false;

            Draw("defaultColor", ref updateColor, "The default color given to the SpriteRenderer.");
            Draw("selectedColor", ref updateColor, "The color given to the SpriteRenderer when selected.");

            if (updateColor)
                Each(tgts, t => t.UpdateColor(), true);
        }
    }
}

#endif