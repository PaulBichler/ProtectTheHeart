using System;
using Lean.Common;
using Lean.Transition;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lean.Gui
{
	/// <summary>
	///     This component allows you to display a tooltip as long as the mouse is hovering over the current UI element, or a
	///     finger is on top.
	///     Tooltips will display for any raycastable UI element that has the <b>LeanTooltipData</b> component.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanTooltip")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Tooltip")]
    public class LeanTooltip : MonoBehaviour
    {
        public enum ActivationType
        {
            HoverOrPress,
            Hover,
            Press
        }

        public enum BoundaryType
        {
            None,
            Pivot,
            Position
        }

        public static PointerEventData HoverPointer;
        public static LeanTooltipData HoverData;
        public static bool HoverShow;

        public static PointerEventData PressPointer;
        public static LeanTooltipData PressData;
        public static bool PressShow;

        private static readonly Vector3[] corners = new Vector3[4];
        [SerializeField] private ActivationType activation;
        [SerializeField] private float showDelay;
        [SerializeField] private bool move = true;
        [SerializeField] private BoundaryType boundary;
        [SerializeField] private LeanPlayer showTransitions;
        [SerializeField] private LeanPlayer hideTransitions;
        [SerializeField] private UnityEventString onShow;
        [SerializeField] private UnityEvent onHide;

        [NonSerialized] private RectTransform cachedRectTransform;

        [NonSerialized] private bool cachedRectTransformSet;

        [NonSerialized] private float currentDelay;

        [NonSerialized] private bool shown;

        [NonSerialized] private LeanTooltipData tooltip;

		/// <summary>
		///     This allows you to control when the tooltip will appear.
		///     HoverOrPress = When the mouse is hovering, or when the mouse/finger is pressing.
		///     Hover = Only when the mouse is hovering.
		///     Press = Only when the mouse/finger is pressing.
		/// </summary>
		public ActivationType Activation
        {
            set => activation = value;
            get => activation;
        }

        /// <summary>This allows you to delay how quickly the tooltip will appear or switch.</summary>
        public float ShowDelay
        {
            set => showDelay = value;
            get => showDelay;
        }

        /// <summary>Move the attached Transform when the tooltip is open?</summary>
        public bool Move
        {
            set => move = value;
            get => move;
        }

        /// <summary>This allows you to control how the tooltip will behave when it goes outside the screen bounds.</summary>
        public BoundaryType Boundary
        {
            set => boundary = value;
            get => boundary;
        }

		/// <summary>
		///     This allows you to perform a transition when this tooltip appears.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		///     NOTE: Any transitions you perform here should be reverted in the <b>Hide Transitions</b> setting using a matching
		///     transition component.
		/// </summary>
		public LeanPlayer ShowTransitions
        {
            get
            {
                if (showTransitions == null) showTransitions = new LeanPlayer();
                return showTransitions;
            }
        }

		/// <summary>
		///     This allows you to perform a transition when this tooltip hides.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		/// </summary>
		public LeanPlayer HideTransitions
        {
            get
            {
                if (hideTransitions == null) hideTransitions = new LeanPlayer();
                return hideTransitions;
            }
        }

        /// <summary>This allows you to perform an action when this tooltip appears.</summary>
        public UnityEventString OnShow
        {
            get
            {
                if (onShow == null) onShow = new UnityEventString();
                return onShow;
            }
        }

        /// <summary>This allows you to perform an action when this tooltip hides.</summary>
        public UnityEvent OnHide
        {
            get
            {
                if (onHide == null) onHide = new UnityEvent();
                return onHide;
            }
        }

        protected virtual void Update()
        {
            if (cachedRectTransformSet == false)
            {
                cachedRectTransform = GetComponent<RectTransform>();
                cachedRectTransformSet = true;
            }

            LeanTooltipData finalData = default(LeanTooltipData);
            Vector2 finalPoint = default(Vector2);

            switch (activation)
            {
                case ActivationType.HoverOrPress:
                {
                    if (HoverShow)
                    {
                        finalData = HoverData;
                        finalPoint = HoverPointer.position;
                    }
                }
                    break;

                case ActivationType.Hover:
                {
                    if (HoverShow && PressShow == false)
                    {
                        finalData = HoverData;
                        finalPoint = HoverPointer.position;
                    }
                }
                    break;

                case ActivationType.Press:
                {
                    if (PressShow && HoverShow && HoverData == PressData)
                    {
                        finalData = PressData;
                        finalPoint = PressPointer.position;
                    }
                }
                    break;
            }

            if (tooltip != finalData)
            {
                currentDelay = 0.0f;
                tooltip = finalData;
                shown = false;

                Hide();
            }

            if (tooltip != null)
            {
                currentDelay += Time.unscaledDeltaTime;

                if (currentDelay >= showDelay)
                {
                    if (shown == false)
                        Show();

                    if (move)
                        cachedRectTransform.position = finalPoint;
                }
            }

            if (move && boundary != BoundaryType.None)
            {
                cachedRectTransform.GetWorldCorners(corners);

                Vector2 min = Vector2.Min(corners[0], Vector2.Min(corners[1], Vector2.Min(corners[2], corners[3])));
                Vector2 max = Vector2.Max(corners[0], Vector2.Max(corners[1], Vector2.Max(corners[2], corners[3])));

                if (boundary == BoundaryType.Pivot)
                {
                    Vector2 pivot = cachedRectTransform.pivot;

                    if (min.x < 0.0f) pivot.x = 0.0f;
                    else if (max.x > Screen.width) pivot.x = 1.0f;
                    if (min.y < 0.0f) pivot.y = 0.0f;
                    else if (max.y > Screen.height) pivot.y = 1.0f;

                    cachedRectTransform.pivot = pivot;
                }

                if (boundary == BoundaryType.Position)
                {
                    Vector3 position = cachedRectTransform.position;

                    if (min.x < 0.0f) position.x -= min.x;
                    else if (max.x > Screen.width) position.x -= max.x - Screen.width;
                    if (min.y < 0.0f) position.y -= min.y;
                    else if (max.y > Screen.height) position.y -= max.y - Screen.height;

                    cachedRectTransform.position = position;
                }
            }
        }

        private void Show()
        {
            shown = true;

            if (showTransitions != null)
                showTransitions.Begin();

            if (onShow != null)
                onShow.Invoke(tooltip.Text);
        }

        private void Hide()
        {
            if (hideTransitions != null)
                hideTransitions.Begin();

            if (onHide != null)
                onHide.Invoke();
        }

        [Serializable]
        public class UnityEventString : UnityEvent<string>
        {
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanTooltip;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanTooltip_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("activation",
                "This allows you to control when the tooltip will appear.\nHoverOrPress = When the mouse is hovering, or when the mouse/finger is pressing.\nHover = Only when the mouse is hovering.\nPress = Only when the mouse/finger is pressing.");
            Draw("showDelay", "This allows you to delay how quickly the tooltip will appear or switch.");

            Separator();

            Draw("move", "Move the attached Transform when the tooltip is open?");
            if (Any(tgts, t => t.Move))
            {
                BeginIndent();
                Draw("boundary",
                    "This allows you to control how the tooltip will behave when it goes outside the screen bounds.");
                EndIndent();
            }

            Separator();

            Draw("showTransitions",
                "This allows you to perform a transition when this tooltip appears. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here should be reverted in the Hide Transitions setting using a matching transition component.");
            Draw("hideTransitions",
                "This allows you to perform a transition when this tooltip hides. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.");

            Separator();

            Draw("onShow");
            Draw("onHide");
        }
    }
}

#endif