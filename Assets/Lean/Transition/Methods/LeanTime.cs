using Lean.Common;
using UnityEditor;
using UnityEngine;

namespace Lean.Transition.Method
{
	/// <summary>
	///     This allows you to change where in the game loop transitions after this will update.
	///     NOTE: Once you submit the previous transitions, this will be reset to default.
	/// </summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanTime")]
    [AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Time" + LeanTransition.MethodsMenuSuffix + "(LeanTime)")]
    public class LeanTime : LeanMethod
    {
        public LeanTiming Update = LeanTiming.Default;

        public override void Register()
        {
            LeanTransition.CurrentTiming = Update;
        }
    }
}

namespace Lean.Transition
{
    public static partial class LeanExtensions
    {
	    /// <summary>
	    ///     This allows you to change where in the game loop transitions after this will update.
	    ///     NOTE: Once you submit the previous transitions, this will be reset to default.
	    /// </summary>
	    public static T TimeTransition<T>(this T target, LeanTiming update)
            where T : Component
        {
            LeanTransition.CurrentTiming = update;
            return target;
        }

	    /// <summary>
	    ///     This allows you to change where in the game loop transitions after this will update.
	    ///     NOTE: Once you submit the previous transitions, this will be reset to default.
	    /// </summary>
	    public static GameObject TimeTransition(this GameObject target, LeanTiming update)
        {
            LeanTransition.CurrentTiming = update;
            return target;
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Transition.Method.Editor
{
    using TARGET = LeanMethod;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanMethod_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("Update");
        }
    }
}

#endif