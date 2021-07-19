﻿using Lean.Common;
using Lean.Transition;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lean.Gui
{
	/// <summary>
	///     This component provides an alternative to Unity's UI button, allowing you to easily add custom transitions, as
	///     well as add an OnDown event.
	/// </summary>
	public abstract class LeanSelectable : Selectable
    {
        [SerializeField] private LeanPlayer interactableTransitions;
        [SerializeField] private LeanPlayer nonInteractableTransitions;
        [SerializeField] private UnityEvent onInteractable;
        [SerializeField] private UnityEvent onNonInteractable;

        [SerializeField] private bool expectedInteractable = true;

        public new bool interactable
        {
            set
            {
                base.interactable = value;

                UpdateInteractable();
            }

            get => base.interactable;
        }

		/// <summary>
		///     This allows you to perform a transition when this element becomes interactable.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		///     NOTE: Any transitions you perform here must be reverted in the <b>Landscape Transitions</b> setting using a
		///     matching transition component.
		/// </summary>
		public LeanPlayer InteractableTransitions
        {
            get
            {
                if (interactableTransitions == null) interactableTransitions = new LeanPlayer();
                return interactableTransitions;
            }
        }

		/// <summary>
		///     This allows you to perform a transition when this element becomes non-interactable.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		///     NOTE: Any transitions you perform here must be reverted in the <b>Landscape Transitions</b> setting using a
		///     matching transition component.
		/// </summary>
		public LeanPlayer NonInteractableTransitions
        {
            get
            {
                if (nonInteractableTransitions == null) nonInteractableTransitions = new LeanPlayer();
                return nonInteractableTransitions;
            }
        }

        public UnityEvent OnInteractable
        {
            get
            {
                if (onInteractable == null) onInteractable = new UnityEvent();
                return onInteractable;
            }
        }

        public UnityEvent OnNonInteractable
        {
            get
            {
                if (onNonInteractable == null) onNonInteractable = new UnityEvent();
                return onNonInteractable;
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            transition = Transition.None;
        }
#endif

        protected override void OnCanvasGroupChanged()
        {
            base.OnCanvasGroupChanged();

            UpdateInteractable();
        }

        private void UpdateInteractable()
        {
            bool currentInteractable = IsInteractable();

            if (currentInteractable != expectedInteractable)
            {
                expectedInteractable = currentInteractable;

                if (expectedInteractable)
                {
                    if (interactableTransitions != null)
                        interactableTransitions.Begin();

                    if (onInteractable != null)
                        onInteractable.Invoke();
                }
                else
                {
                    if (nonInteractableTransitions != null)
                        nonInteractableTransitions.Begin();

                    if (onNonInteractable != null)
                        onNonInteractable.Invoke();
                }
            }
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanSelectable;

    public class LeanSelectable_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            DrawSelectableSettings();

            Separator();

            bool showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

            Separator();

            DrawSelectableTransitions(showUnusedEvents);

            Separator();

            DrawSelectableEvents(showUnusedEvents);
        }

        protected virtual void DrawSelectableSettings()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            if (Draw("m_Interactable"))
                Each(tgts,
                    t =>
                        t.interactable =
                            serializedObject.FindProperty("expectedInteractable").boolValue =
                                serializedObject.FindProperty("m_Interactable").boolValue, true);
            Draw("m_Transition");

            if (Any(tgts, t => t.transition == Selectable.Transition.ColorTint))
            {
                BeginIndent();
                Draw("m_TargetGraphic");
                Draw("m_Colors");
                EndIndent();
            }

            if (Any(tgts, t => t.transition == Selectable.Transition.SpriteSwap))
            {
                BeginIndent();
                Draw("m_TargetGraphic");
                Draw("m_SpriteState");
                EndIndent();
            }

            if (Any(tgts, t => t.transition == Selectable.Transition.Animation))
            {
                BeginIndent();
                Draw("m_AnimationTriggers");
                EndIndent();
            }

            Draw("m_Navigation");
        }

        protected virtual void DrawSelectableTransitions(bool showUnusedEvents)
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            if (showUnusedEvents || Any(tgts, t => t.InteractableTransitions.IsUsed))
                Draw("interactableTransitions");

            if (showUnusedEvents || Any(tgts, t => t.NonInteractableTransitions.IsUsed))
                Draw("nonInteractableTransitions");
        }

        protected virtual void DrawSelectableEvents(bool showUnusedEvents)
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            if (showUnusedEvents || Any(tgts, t => t.OnInteractable.GetPersistentEventCount() > 0))
                Draw("onInteractable");

            if (showUnusedEvents || Any(tgts, t => t.OnNonInteractable.GetPersistentEventCount() > 0))
                Draw("onNonInteractable");
        }
    }
}

#endif