using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;
using UnityEngine.Serialization;
using TARGET = UnityEngine.RectTransform;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to transition the RectTransform's pivot.y value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanRectTransformPivot_y")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "RectTransform/RectTransform.pivot.y" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanRectTransformPivot_y)")]
    public class LeanRectTransformPivot_y : LeanMethodWithStateAndTarget
    {
        public State Data;

        public override Type GetTargetType()
        {
            return typeof(TARGET);
        }

        public override void Register()
        {
            PreviousState = Register(GetAliasedTarget(Data.Target), Data.Value, Data.Duration, Data.Ease);
        }

        public static LeanState Register(TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
        {
            State state = LeanTransition.SpawnWithTarget(State.Pool, target);

            state.Value = value;

            state.Ease = ease;

            return LeanTransition.Register(state, duration);
        }

        [Serializable]
        public class State : LeanStateWithTarget<TARGET>
        {
            public static Stack<State> Pool = new Stack<State>();

            [Tooltip("The pivot value will transition to this.")] [FormerlySerializedAs("Pivot")] public float Value;

            [Tooltip("This allows you to control how the transition will look.")] public LeanEase Ease =
                LeanEase.Smooth;

            [NonSerialized] private float oldValue;

            public override int CanFill => Target != null && Target.pivot.y != Value ? 1 : 0;

            public override void FillWithTarget()
            {
                Value = Target.pivot.y;
            }

            public override void BeginWithTarget()
            {
                oldValue = Target.pivot.y;
            }

            public override void UpdateWithTarget(float progress)
            {
                Vector2 vector = Target.pivot;

                vector.y = Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));

                Target.pivot = vector;
            }

            public override void Despawn()
            {
                Pool.Push(this);
            }
        }
    }
}

namespace Lean.Transition
{
    public static partial class LeanExtensions
    {
        public static TARGET pivotTransition_y(this TARGET target, float value, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanRectTransformPivot_y.Register(target, value, duration, ease);
            return target;
        }
    }
}