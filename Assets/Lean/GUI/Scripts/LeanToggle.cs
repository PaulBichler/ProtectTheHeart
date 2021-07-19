using System;
using System.Collections.Generic;
using Lean.Common;
using Lean.Transition;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Gui
{
	/// <summary>
	///     This component allows you to make a UI element that can switch between two states.
	///     For example, you could create a panel that shows and hides using this component.
	///     NOTE: If you want to create windows that toggle open and closed, then I recommend you instead use the Lean Window
	///     component. This works identically, but it also registers with the Lean Window Closer component, allowing you to
	///     close them in sequence.
	/// </summary>
	[ExecuteInEditMode]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanToggle")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Toggle")]
    public class LeanToggle : MonoBehaviour
    {
        /// <summary>This stores all active and enabled LeanToggle instances.</summary>
        public static LinkedList<LeanToggle> Instances = new LinkedList<LeanToggle>();

        [SerializeField] private bool on;
        [SerializeField] private bool turnOffSiblings;
        [SerializeField] private LeanPlayer onTransitions;
        [SerializeField] private LeanPlayer offTransitions;
        [SerializeField] private UnityEvent onOn;
        [SerializeField] private UnityEvent onOff;

        [NonSerialized] private LinkedListNode<LeanToggle> link;

        /// <summary>This lets you change the current toggle state of this UI element.</summary>
        public bool On
        {
            set => Set(value);
            get => on;
        }

		/// <summary>
		///     If you enable this, then any sibling GameObjects (i.e. same parent GameObject) will automatically be turned off.
		///     This allows you to make radio boxes, or force only one panel to show at a time, etc.
		/// </summary>
		public bool TurnOffSiblings
        {
            set
            {
                if (turnOffSiblings = value) TurnOffSiblingsNow();
            }
            get => turnOffSiblings;
        }

		/// <summary>
		///     This allows you to perform a transition when this toggle turns on.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		///     NOTE: Any transitions you perform here should be reverted in the <b>Off Transitions</b> setting using a matching
		///     transition component.
		/// </summary>
		public LeanPlayer OnTransitions
        {
            get
            {
                if (onTransitions == null) onTransitions = new LeanPlayer();
                return onTransitions;
            }
        }

		/// <summary>
		///     This allows you to perform a transition when this toggle turns off.
		///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
		///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
		/// </summary>
		public LeanPlayer OffTransitions
        {
            get
            {
                if (offTransitions == null) offTransitions = new LeanPlayer();
                return offTransitions;
            }
        }

        /// <summary>This allows you to perform an action when this toggle turns on.</summary>
        public UnityEvent OnOn
        {
            get
            {
                if (onOn == null) onOn = new UnityEvent();
                return onOn;
            }
        }

        /// <summary>This allows you to perform an action when this toggle turns off.</summary>
        public UnityEvent OnOff
        {
            get
            {
                if (onOff == null) onOff = new UnityEvent();
                return onOff;
            }
        }

        protected virtual void OnEnable()
        {
            link = Instances.AddLast(this);
        }

        protected virtual void OnDisable()
        {
            Instances.Remove(link);

            link = null;
        }

        /// <summary>This allows you to set the toggle on or off.</summary>
        public void Set(bool value)
        {
            if (value)
                TurnOn();
            else
                TurnOff();
        }

        /// <summary>This allows you to toggle the state of this toggle (i.e. if it's turned on, then this will turn it off).</summary>
        [ContextMenu("Toggle")]
        public void Toggle()
        {
            On = !On;
        }

        /// <summary>If this toggle is turned off, then this will turn it on.</summary>
        [ContextMenu("Turn On")]
        public void TurnOn()
        {
            if (on == false)
            {
                on = true;

                TurnOnNow();
            }
        }

        [ContextMenu("Turn Off")]
        public void TurnOff()
        {
            if (@on)
            {
                on = false;

                TurnOffNow();
            }
        }

        /// <summary>This will search for any sibling toggles (i.e. they have the same parent GameObject), and turn them off.</summary>
        [ContextMenu("Turn Off Siblings Now")]
        public void TurnOffSiblingsNow()
        {
            Transform parent = transform.parent;

            if (parent != null)
            {
                int ignore = transform.GetSiblingIndex();

                for (int i = parent.childCount - 1; i >= 0; i--)
                    if (i != ignore)
                    {
                        LeanToggle sibling = parent.GetChild(i).GetComponent<LeanToggle>();

                        if (sibling != null)
                            sibling.TurnOff();
                    }
            }
        }

        /// <summary>This will go through every toggle in the scene except this one, and turn them off.</summary>
        [ContextMenu("Turn Off Others Now")]
        public void TurnOffOthersNow()
        {
            LinkedListNode<LeanToggle> node = Instances.First;

            for (int i = Instances.Count - 1; i >= 0; i--)
            {
                LeanToggle instance = node.Value;

                if (instance != this)
                    instance.TurnOff();

                node = node.Next;
            }
        }

		/// <summary>
		///     This will return true if all the active and enabled toggle instances with the specified GameObject name are
		///     turned on.
		/// </summary>
		public static bool AllOn(string name)
        {
            LinkedListNode<LeanToggle> node = Instances.First;
            bool on = false;

            for (int i = Instances.Count - 1; i >= 0; i--)
            {
                LeanToggle instance = node.Value;

                if (instance.name == name)
                {
                    if (instance.on == false)
                        return false;

                    on = true;
                }

                node = node.Next;
            }

            return on;
        }

		/// <summary>
		///     This will return true if all the active and enabled toggle instances with the specified GameObject name are
		///     turned off.
		/// </summary>
		public static bool AllOff(string name)
        {
            LinkedListNode<LeanToggle> node = Instances.First;
            bool off = false;

            for (int i = Instances.Count - 1; i >= 0; i--)
            {
                LeanToggle instance = node.Value;

                if (instance.name == name)
                {
                    if (instance.@on)
                        return false;

                    off = true;
                }

                node = node.Next;
            }

            return off;
        }

		/// <summary>
		///     This allows you to set the <b>On</b> state of all the active and enabled toggle instances with the specified
		///     GameObject name.
		/// </summary>
		public static void SetAll(string name, bool on)
        {
            LinkedListNode<LeanToggle> node = Instances.First;

            for (int i = Instances.Count - 1; i >= 0; i--)
            {
                LeanToggle instance = node.Value;

                if (instance.name == name)
                    instance.On = @on;

                node = node.Next;
            }
        }

		/// <summary>
		///     This allows you to toggle the state (i.e. if it's turned on, then this will turn it off) of all active and
		///     enabled toggle instances with the specified GameObject name.
		/// </summary>
		public static void ToggleAll(string name)
        {
            LinkedListNode<LeanToggle> node = Instances.First;

            for (int i = Instances.Count - 1; i >= 0; i--)
            {
                LeanToggle instance = node.Value;

                if (instance.name == name)
                    instance.Toggle();

                node = node.Next;
            }
        }

        /// <summary>This allows you to turn on every active and enabled toggle instance with the specified GameObject name.</summary>
        public static void TurnOnAll(string name)
        {
            LinkedListNode<LeanToggle> node = Instances.First;

            for (int i = Instances.Count - 1; i >= 0; i--)
            {
                LeanToggle instance = node.Value;

                if (instance.name == name)
                    instance.TurnOn();

                node = node.Next;
            }
        }

        /// <summary>This allows you to turn off each active and enabled LeanToggle instance with the specified GameObject name.</summary>
        public static void TurnOffAll(string name)
        {
            LinkedListNode<LeanToggle> node = Instances.First;

            for (int i = Instances.Count - 1; i >= 0; i--)
            {
                LeanToggle instance = node.Value;

                if (instance.name == name)
                    instance.TurnOff();

                node = node.Next;
            }
        }

        protected virtual void TurnOnNow()
        {
            if (turnOffSiblings)
                TurnOffSiblingsNow();

            if (onTransitions != null)
                onTransitions.Begin();

            if (onOn != null)
                onOn.Invoke();
        }

        protected virtual void TurnOffNow()
        {
            if (offTransitions != null)
                offTransitions.Begin();

            if (onOff != null)
                onOff.Invoke();
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanToggle;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanToggle_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            if (Draw("on", "This lets you change the current toggle state of this UI element."))
                Each(tgts, t => t.On = serializedObject.FindProperty("on").boolValue, true);

            if (Draw("turnOffSiblings",
                "If you enable this, then any sibling GameObjects (i.e. same parent GameObject) will automatically be turned off. This allows you to make radio boxes, or force only one panel to show at a time, etc."))
                Each(tgts, t => t.TurnOffSiblings = serializedObject.FindProperty("turnOffSiblings").boolValue, true);

            Separator();

            Draw("onTransitions",
                "This allows you to perform a transition when this toggle turns on. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the Graphic.color Transition (LeanGraphicColor) component can be used to change the color.\n\nNOTE: Any transitions you perform here should be reverted in the <b>Off Transitions</b> setting using a matching transition component.");
            Draw("offTransitions",
                "This allows you to perform a transition when this toggle turns off. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the Graphic.color Transition (LeanGraphicColor) component can be used to change the color.");

            Separator();

            Draw("onOn");
            Draw("onOff");
        }
    }
}

#endif