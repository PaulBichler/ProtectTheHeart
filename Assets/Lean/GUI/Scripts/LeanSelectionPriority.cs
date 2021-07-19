using Lean.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lean.Gui
{
	/// <summary>
	///     This component marks the current GameObject as being high priority for selection.
	///     This setting is used by the <b>LeanSelectionManager</b> when decided what to select.
	/// </summary>
	[RequireComponent(typeof(Selectable))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanSelectionPriority")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Selection Priority")]
    public class LeanSelectionPriority : MonoBehaviour
    {
        [SerializeField] private float priority = 100.0f;

        /// <summary>This allows you to set the priority of this GameObject among others when being automatically selected.</summary>
        public float Priority
        {
            set => priority = value;
            get => priority;
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanSelectionPriority;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanSelectionPriority_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("priority",
                "This allows you to set the priority of this GameObject among others when being automatically selected.");
        }
    }
}

#endif