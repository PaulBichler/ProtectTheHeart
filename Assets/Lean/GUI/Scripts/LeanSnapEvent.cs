using Lean.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Gui
{
	/// <summary>
	///     This component allows you to fire an events when the <b>LeanSnap</b> component's <b>Position</b> is at
	///     specific values.
	/// </summary>
	[ExecuteInEditMode]
    [RequireComponent(typeof(LeanSnap))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanSnapEvent")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Snap Event")]
    public class LeanSnapEvent : MonoBehaviour
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private UnityEvent onAction;

        /// <summary>The <b>LeanSnap.Position</b> you want to listen for.</summary>
        public Vector2Int Position
        {
            set => position = value;
            get => position;
        }

        /// <summary>The action that will be invoked.</summary>
        public UnityEvent OnAction
        {
            get
            {
                if (onAction == null) onAction = new UnityEvent();
                return onAction;
            }
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanSnapEvent;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanSnapEvent_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("position", "The LeanSnap.Position you want to listen for.");
            Draw("onAction");
        }
    }
}

#endif