using System;
using Lean.Transition;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lean.Gui
{
    /// <summary>This component allows you to drag the specified RectTransform when you drag on this UI element.</summary>
    [RequireComponent(typeof(RectTransform))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanDrag")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Drag")]
    public class LeanDrag : LeanSelectable, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private bool horizontal = true;
        [SerializeField] private bool vertical = true;
        [SerializeField] private LeanPlayer beginTransitions;
        [SerializeField] private LeanPlayer endTransitions;
        [SerializeField] private UnityEvent onBegin;
        [SerializeField] private UnityEvent onEnd;

        [NonSerialized] private RectTransform cachedRectTransform;

        [NonSerialized] private bool cachedRectTransformSet;

        [NonSerialized] private Vector2 currentPosition;

        // Is this element currently being dragged?
        [NonSerialized] protected bool dragging;

	    /// <summary>
	    ///     If you want a different RectTransform to be moved while dragging on this UI element, then specify it here.
	    ///     This allows you to turn the current UI element into a drag handle.
	    /// </summary>
	    public RectTransform Target
        {
            set => target = value;
            get => target;
        }

        /// <summary>Should you be able to drag horizontally?</summary>
        public bool Horizontal
        {
            set => horizontal = value;
            get => horizontal;
        }

        /// <summary>Should you be able to drag vertically?</summary>
        public bool Vertical
        {
            set => vertical = value;
            get => vertical;
        }

	    /// <summary>
	    ///     This allows you to perform a transition when this element begins being dragged.
	    ///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
	    ///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
	    ///     NOTE: Any transitions you perform here must be reverted in the <b>Normal Transitions</b> setting using a matching
	    ///     transition component.
	    /// </summary>
	    public LeanPlayer BeginTransitions
        {
            get
            {
                if (beginTransitions == null) beginTransitions = new LeanPlayer();
                return beginTransitions;
            }
        }

	    /// <summary>
	    ///     This allows you to perform a transition when this element ends being dragged.
	    ///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
	    ///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
	    /// </summary>
	    public LeanPlayer EndTransitions
        {
            get
            {
                if (endTransitions == null) endTransitions = new LeanPlayer();
                return endTransitions;
            }
        }

        /// <summary>This allows you to perform an actions when this element begins being dragged.</summary>
        public UnityEvent OnBegin
        {
            get
            {
                if (onBegin == null) onBegin = new UnityEvent();
                return onBegin;
            }
        }

        /// <summary>This allows you to perform an actions when this element ends being dragged.</summary>
        public UnityEvent OnEnd
        {
            get
            {
                if (onEnd == null) onEnd = new UnityEvent();
                return onEnd;
            }
        }

        /// <summary>This will return true if the mouse/finger is currently dragging this UI element.</summary>
        public bool Dragging => dragging;

        public RectTransform TargetTransform
        {
            get
            {
                if (target != null)
                    return target;

                if (cachedRectTransformSet == false)
                {
                    cachedRectTransform = GetComponent<RectTransform>();
                    cachedRectTransformSet = true;
                }

                return cachedRectTransform;
            }
        }

        protected override void Start()
        {
            base.Start();

            transition = Transition.None;
            interactable = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            LeanGui.OnDraggingCheck += DraggingCheck;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            LeanGui.OnDraggingCheck -= DraggingCheck;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Only allow dragging if certain conditions are met
            if (MayDrag(eventData))
            {
                Vector2 vector = default;
                RectTransform target = TargetTransform;

                // Is this pointer inside this rect transform?
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position,
                    eventData.pressEventCamera, out vector))
                {
                    dragging = true;
                    currentPosition = target.anchoredPosition;

                    if (beginTransitions != null)
                        beginTransitions.Begin();

                    if (onBegin != null)
                        onBegin.Invoke();
                }
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            // Only drag if OnBeginDrag was successful
            if (dragging)
                // Only allow dragging if certain conditions are met
                if (MayDrag(eventData))
                {
                    Vector2 oldVector = default;
                    RectTransform target = TargetTransform;

                    // Get the previous pointer position relative to this rect transform
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target,
                        eventData.position - eventData.delta, eventData.pressEventCamera, out oldVector))
                    {
                        Vector2 newVector = default;

                        // Get the current pointer position relative to this rect transform
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position,
                            eventData.pressEventCamera, out newVector))
                        {
                            Vector2 anchoredPosition = target.anchoredPosition;

                            currentPosition += (Vector2) (target.localRotation * (newVector - oldVector));

                            if (horizontal)
                                anchoredPosition.x = currentPosition.x;

                            if (vertical)
                                anchoredPosition.y = currentPosition.y;

                            // Offset the anchored position by the difference
                            target.anchoredPosition = anchoredPosition;
                        }
                    }
                }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;

            if (endTransitions != null)
                endTransitions.Begin();

            if (onEnd != null)
                onEnd.Invoke();
        }

        private void DraggingCheck(ref bool check)
        {
            if (dragging)
                check = true;
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable(); // && eventData.button == PointerEventData.InputButton.Left;
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanDrag;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanDrag_Editor : LeanSelectable_Editor
    {
        protected override void DrawSelectableSettings()
        {
            base.DrawSelectableSettings();

            Draw("target",
                "If you want a different RectTransform to be moved while dragging on this UI element, then specify it here. This allows you to turn the current UI element into a drag handle.");

            Separator();

            Draw("horizontal", "Should you be able to drag horizontally?");
            Draw("vertical", "Should you be able to drag vertically?");
        }

        protected override void DrawSelectableTransitions(bool showUnusedEvents)
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            if (showUnusedEvents || Any(tgts, t => t.BeginTransitions.IsUsed))
                Draw("beginTransitions",
                    "This allows you to perform a transition when this element begins being dragged. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here must be reverted in the Normal Transitions setting using a matching transition component.");

            if (showUnusedEvents || Any(tgts, t => t.EndTransitions.IsUsed))
                Draw("endTransitions",
                    "This allows you to perform a transition when this element ends being dragged. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.");

            base.DrawSelectableTransitions(showUnusedEvents);
        }

        protected override void DrawSelectableEvents(bool showUnusedEvents)
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            if (showUnusedEvents || Any(tgts, t => t.OnBegin.GetPersistentEventCount() > 0))
                Draw("onBegin");

            if (showUnusedEvents || Any(tgts, t => t.OnEnd.GetPersistentEventCount() > 0))
                Draw("onEnd");

            base.DrawSelectableEvents(showUnusedEvents);
        }
    }
}

#endif