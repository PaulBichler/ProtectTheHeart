using System;
using Lean.Common;
using Lean.Transition;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lean.Gui
{
    /// <summary>This component allows you to detect when a finger swipes over the current RectTransform.</summary>
    [RequireComponent(typeof(RectTransform))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanSwipe")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Swipe")]
    public class LeanSwipe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float minimumDistance = 100.0f;
        [SerializeField] private float maximumTime = 0.125f;
        [SerializeField] private bool checkAngle;
        [SerializeField] private float desiredAngle;
        [Range(0.0f, 360.0f)] [SerializeField] private float maximumRange = 90.0f;
        [SerializeField] private LeanPlayer swipeTransitions;
        [SerializeField] private UnityEvent onSwipe;

        [NonSerialized] private RectTransform cachedRectTransform;

        [NonSerialized] private bool cachedRectTransformSet;

        [NonSerialized] protected Vector2 dragDelta;

        [NonSerialized] protected bool dragging;

        [NonSerialized] protected float dragTime;

        /// <summary>The swiping finger must move at least this many pixels.</summary>
        public float MinimumDistance
        {
            set => minimumDistance = value;
            get => minimumDistance;
        }

        /// <summary>The swiping finger must move MinimumDistance within this amount of seconds.</summary>
        public float MaximumTime
        {
            set => maximumTime = value;
            get => maximumTime;
        }

        /// <summary>If you need the swipe to be in a specific direction, then enable this.</summary>
        public bool CheckAngle
        {
            set => checkAngle = value;
            get => checkAngle;
        }

	    /// <summary>
	    ///     0 = up
	    ///     90 = right
	    ///     180 = down
	    ///     270 = left
	    /// </summary>
	    public float DesiredAngle
        {
            set => desiredAngle = value;
            get => desiredAngle;
        }

	    /// <summary>
	    ///     360 = full circle
	    ///     180 = half circle
	    ///     90 = quarter circle
	    /// </summary>
	    public float MaximumRange
        {
            set => maximumRange = value;
            get => maximumRange;
        }

	    /// <summary>
	    ///     This allows you to perform a transition when you swipe this UI element.
	    ///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
	    ///     For example, the <b>LeanPlaySound (Play Sound Transition)</b> component can be used to play a click sound.
	    /// </summary>
	    public LeanPlayer SwipeTransitions
        {
            get
            {
                if (swipeTransitions == null) swipeTransitions = new LeanPlayer();
                return swipeTransitions;
            }
        }

        /// <summary>This allows you to perform an action when you swipe this UI element.</summary>
        public UnityEvent OnSwipe
        {
            get
            {
                if (onSwipe == null) onSwipe = new UnityEvent();
                return onSwipe;
            }
        }

        public RectTransform CachedRectTransform
        {
            get
            {
                if (cachedRectTransformSet == false)
                {
                    cachedRectTransform = GetComponent<RectTransform>();
                    cachedRectTransformSet = true;
                }

                return cachedRectTransform;
            }
        }

        protected virtual void Update()
        {
            if (dragging)
            {
                dragTime += Time.deltaTime;

                if (dragTime <= maximumTime && dragDelta.magnitude >= minimumDistance)
                {
                    // If this swipe angle is incorrect, return
                    if (checkAngle)
                    {
                        float angle = Mathf.Atan2(dragDelta.x, dragDelta.y) * Mathf.Rad2Deg;
                        float delta = Mathf.DeltaAngle(angle, desiredAngle) * 2.0f;

                        if (delta <= -maximumRange || delta > maximumRange)
                            return;
                    }

                    // Cancel drag and invoke
                    dragging = false;

                    if (swipeTransitions != null)
                        swipeTransitions.Begin();

                    if (onSwipe != null)
                        onSwipe.Invoke();
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector2 vector = default(Vector2);

            // Is this pointer inside this rect transform?
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(CachedRectTransform, eventData.position,
                eventData.pressEventCamera, out vector))
            {
                dragging = true;
                dragTime = 0.0f;
                dragDelta = Vector2.zero;
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (dragging)
                dragDelta += eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanSwipe;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanSwipe_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            BeginError(Any(tgts, t => t.MinimumDistance <= 0.0f));
            Draw("minimumDistance", "The swiping finger must move at least this many pixels.");
            EndError();
            BeginError(Any(tgts, t => t.MaximumTime <= 0.0f));
            Draw("maximumTime", "The swiping finger must move MinimumDistance within this amount of seconds.");
            EndError();
            Draw("checkAngle", "If you need the swipe to be in a specific direction, then enable this.");
            if (Any(tgts, t => t.CheckAngle))
            {
                BeginIndent();
                Draw("desiredAngle", "0 = up\n\n90 = right\n\n180 = down\n\n270 = left");
                BeginError(Any(tgts, t => t.MaximumRange <= 0.0f && t.MaximumRange >= 360.0f));
                Draw("maximumRange", "360 = full circle\n\n180 = half circle\n\n 90 = quarter circle");
                EndError();
                EndIndent();
            }

            Separator();

            Draw("swipeTransitions",
                "This allows you to perform a transition when you swipe this UI element. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the LeanPlaySound (Play Sound Transition) component can be used to play a click sound.");

            Separator();

            Draw("onSwipe");
        }
    }
}

#endif