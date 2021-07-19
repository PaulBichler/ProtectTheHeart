using System;
using System.Collections.Generic;
using Lean.Common;
using Lean.Transition;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lean.Gui
{
	/// <summary>
	///     This component allows you to perform an action as long as the mouse is hovering over the current UI element,
	///     or a finger is on top.
	/// </summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanHover")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Hover")]
    public class LeanHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool multiEnter;
        [SerializeField] private bool multiExit;
        [SerializeField] private LeanPlayer enterTransitions;
        [SerializeField] private LeanPlayer exitTransitions;
        [SerializeField] private UnityEvent onEnter;
        [SerializeField] private UnityEvent onExit;

        /// <summary>Track the currently down pointers so we can toggle the transition.</summary>
        [NonSerialized] private readonly List<int> enteredPointers = new List<int>();

		/// <summary>
		///     Enable this if you want EnterTransitions + OnEnter to be invoked once for each mouse/finger that enters this
		///     element.
		/// </summary>
		public bool MultiEnter
        {
            set => multiEnter = value;
            get => multiEnter;
        }

		/// <summary>
		///     Enable this if you want ExitTransitions + OnExit to be invoked once for each mouse/finger that exits this
		///     element.
		/// </summary>
		public bool MultiExit
        {
            set => multiExit = value;
            get => multiExit;
        }

		/// <summary>
		///     This allows you to perform a transition when the mouse/finger enters this UI element.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		///     NOTE: Any transitions you perform here must be reverted in the <b>Exit Transitions</b> setting using a matching
		///     transition component.
		/// </summary>
		public LeanPlayer EnterTransitions
        {
            get
            {
                if (enterTransitions == null) enterTransitions = new LeanPlayer();
                return enterTransitions;
            }
        }

		/// <summary>
		///     This allows you to perform a transition when the mouse/finger exits this UI element.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		/// </summary>
		public LeanPlayer ExitTransitions
        {
            get
            {
                if (exitTransitions == null) exitTransitions = new LeanPlayer();
                return exitTransitions;
            }
        }

        /// <summary>This allows you to perform an action when the mouse/finger enters this UI element.</summary>
        public UnityEvent OnEnter
        {
            get
            {
                if (onEnter == null) onEnter = new UnityEvent();
                return onEnter;
            }
        }

        /// <summary>This allows you to perform an action when the mouse/finger exits this UI element.</summary>
        public UnityEvent OnExit
        {
            get
            {
                if (onExit == null) onExit = new UnityEvent();
                return onExit;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                enteredPointers.Add(eventData.pointerId);

                if (MultiEnter == false && enteredPointers.Count > 1)
                    return;

                if (enterTransitions != null)
                    enterTransitions.Begin();

                if (onEnter != null)
                    onEnter.Invoke();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (enteredPointers.Remove(eventData.pointerId))
            {
                if (MultiExit == false && enteredPointers.Count > 0)
                    return;

                if (exitTransitions != null)
                    exitTransitions.Begin();

                if (onExit != null)
                    onExit.Invoke();
            }
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanHover;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanHover_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("multiEnter",
                "Enable this if you want EnterTransitions + OnEnter to be invoked once for each mouse/finger that enters this element.");
            Draw("multiExit",
                "Enable this if you want ExitTransitions + OnExit to be invoked once for each mouse/finger that exits this element.");

            Separator();

            Draw("enterTransitions",
                "This allows you to perform a transition when the mouse/finger enters this UI element. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the Graphic.color Transition (LeanGraphicColor) component can be used to change the color.\n\nNOTE: Any transitions you perform here must be reverted in the Exit Transitions setting using a matching transition component.");
            Draw("exitTransitions",
                "This allows you to perform a transition when the mouse/finger exits this UI element. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the Graphic.color Transition (LeanGraphicColor) component can be used to change the color.");

            Separator();

            Draw("onEnter", "This allows you to perform an action when the mouse/finger enters this UI element.");
            Draw("onExit", "This allows you to perform an action when the mouse/finger exits this UI element.");
        }
    }
}

#endif