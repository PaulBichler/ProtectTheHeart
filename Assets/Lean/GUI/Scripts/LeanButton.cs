﻿using System;
using System.Collections.Generic;
using Lean.Transition;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lean.Gui
{
	/// <summary>
	///     This component provides an alternative to Unity's UI button, allowing you to easily add custom transitions, as
	///     well as add an OnDown event.
	/// </summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanButton")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Button")]
    public class LeanButton : LeanSelectable, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
        ISubmitHandler
    {
        [SerializeField] private bool multiDown;
        [SerializeField] private float dragThreshold = 10.0f;
        [SerializeField] private LeanPlayer normalTransitions;
        [SerializeField] private LeanPlayer downTransitions;
        [SerializeField] private LeanPlayer clickTransitions;
        [SerializeField] private UnityEvent onDown;
        [SerializeField] private UnityEvent onClick;

        /// <summary>Track the currently down pointers so we can toggle the transition.</summary>
        private readonly List<int> downPointers = new List<int>();

        [NonSerialized] private LeanDrag parentDrag;

        [NonSerialized] private ScrollRect parentScrollRect;

        [NonSerialized] private Vector2 totalDelta;

		/// <summary>
		///     If you enable this then OnDown + DownTransition be invoked once for each finger that begins touching the button.
		///     If you disable this then OnDown + DownTransition will only be invoked for the first finger that begins touching the
		///     button.
		/// </summary>
		public bool MultiDown
        {
            set => multiDown = value;
            get => multiDown;
        }

		/// <summary>
		///     If your finger presses down on the button and drags more than this many pixels, then selection will be canceled.
		///     -1 = Unlimited drag distance.
		///     0 = Until the finger exits the button graphic.
		/// </summary>
		public float DragThreshold
        {
            set => dragThreshold = value;
            get => dragThreshold;
        }

		/// <summary>
		///     This allows you to perform a transition when there are no longer any fingers touching the button.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color back
		///     to normal.
		/// </summary>
		public LeanPlayer NormalTransitions
        {
            get
            {
                if (normalTransitions == null) normalTransitions = new LeanPlayer();
                return normalTransitions;
            }
        }

		/// <summary>
		///     This allows you to perform a transition when a finger begins touching the button.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		///     NOTE: Any transitions you perform here must be reverted in the <b>Normal Transitions</b> setting using a matching
		///     transition component.
		/// </summary>
		public LeanPlayer DownTransitions
        {
            get
            {
                if (downTransitions == null) downTransitions = new LeanPlayer();
                return downTransitions;
            }
        }

		/// <summary>
		///     This allows you to perform a transition when you click or tap on this button.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Play Sound Transition (LeanPlaySound)</b> component can be used to play a click sound.
		/// </summary>
		public LeanPlayer ClickTransitions
        {
            get
            {
                if (clickTransitions == null) clickTransitions = new LeanPlayer();
                return clickTransitions;
            }
        }

        /// <summary>This allows you to perform an action when a finger begins touching the button.</summary>
        public UnityEvent OnDown
        {
            get
            {
                if (onDown == null) onDown = new UnityEvent();
                return onDown;
            }
        }

        /// <summary>This allows you to perform an action when you click or tap on this button.</summary>
        public UnityEvent OnClick
        {
            get
            {
                if (onClick == null) onClick = new UnityEvent();
                return onClick;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            parentScrollRect = GetComponentInParent<ScrollRect>();
            parentDrag = GetComponentInParent<LeanDrag>();

            if (parentScrollRect != null)
                parentScrollRect.OnBeginDrag(eventData);

            if (parentDrag != null)
                parentDrag.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (downPointers.Contains(eventData.pointerId))
            {
                totalDelta += eventData.delta;

                if (dragThreshold > 0.0f && totalDelta.magnitude > dragThreshold)
                {
                    downPointers.Remove(eventData.pointerId);

                    TryNormal();
                }
            }

            if (parentScrollRect != null)
                parentScrollRect.OnDrag(eventData);

            if (parentDrag != null)
                parentDrag.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (parentScrollRect != null)
                parentScrollRect.OnEndDrag(eventData);

            if (parentDrag != null)
                parentDrag.OnEndDrag(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (IsInteractable() && eventData.button == PointerEventData.InputButton.Left)
            {
                totalDelta = Vector2.zero;

                downPointers.Add(eventData.pointerId);

                if (multiDown || downPointers.Count == 1)
                {
                    if (downTransitions != null)
                        downTransitions.Begin();

                    if (onDown != null)
                        onDown.Invoke();
                }
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (enabled)
                DoClick();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (downPointers.Remove(eventData.pointerId))
            {
                TryNormal();
                DoClick();
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (dragThreshold == 0.0f)
            {
                downPointers.Remove(eventData.pointerId);

                TryNormal();
            }
        }

        private void TryNormal()
        {
            if (downPointers.Count == 0)
                if (normalTransitions != null)
                    normalTransitions.Begin();
        }

        private void DoClick()
        {
            if (clickTransitions != null)
                clickTransitions.Begin();

            if (onClick != null)
                onClick.Invoke();
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanButton;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanButton_Editor : LeanSelectable_Editor
    {
        protected override void DrawSelectableSettings()
        {
            base.DrawSelectableSettings();

            Draw("dragThreshold",
                "If your finger presses down on the button and drags more than this many pixels, then selection will be canceled.\n\n-1 = Unlimited drag distance.\n\n0 = Until the finger exits the button graphic.");
            Draw("multiDown",
                "If you press multiple fingers on this button at the same time, should OnDown and DownTransition be invoked multiple times?");
        }

        protected override void DrawSelectableTransitions(bool showUnusedEvents)
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            if (showUnusedEvents || Any(tgts, t => t.NormalTransitions.IsUsed))
                Draw("normalTransitions");

            if (showUnusedEvents || Any(tgts, t => t.DownTransitions.IsUsed))
                Draw("downTransitions");

            if (showUnusedEvents || Any(tgts, t => t.ClickTransitions.IsUsed))
                Draw("clickTransitions");

            base.DrawSelectableTransitions(showUnusedEvents);
        }

        protected override void DrawSelectableEvents(bool showUnusedEvents)
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            if (showUnusedEvents || Any(tgts, t => t.OnDown.GetPersistentEventCount() > 0))
                Draw("onDown");

            if (showUnusedEvents || Any(tgts, t => t.OnClick.GetPersistentEventCount() > 0))
                Draw("onClick");

            base.DrawSelectableEvents(showUnusedEvents);
        }
    }
}

#endif