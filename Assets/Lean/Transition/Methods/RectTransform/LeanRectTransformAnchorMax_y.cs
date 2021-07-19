using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;
using TARGET = UnityEngine.RectTransform;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to transition the RectTransform's anchorMax.y value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanRectTransformAnchorMax_y")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "RectTransform/RectTransform.anchorMax.y" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanRectTransformAnchorMax_y)")]
    public class LeanRectTransformAnchorMax_y : LeanMethodWithStateAndTarget
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

            [Tooltip("The anchorMax value will transition to this.")] public float Value;

            [Tooltip("This allows you to control how the transition will look.")] public LeanEase Ease =
                LeanEase.Smooth;

            [NonSerialized] private float oldValue;

            public override int CanFill => Target != null && Target.anchorMax.y != Value ? 1 : 0;

            public override void FillWithTarget()
            {
                Value = Target.anchorMax.y;
            }

            public override void BeginWithTarget()
            {
                oldValue = Target.anchorMax.y;
            }

            public override void UpdateWithTarget(float progress)
            {
                Vector2 vector = Target.anchorMax;

                vector.y = Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));

                Target.anchorMax = vector;
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
        public static TARGET anchorMaxTransition_y(this TARGET target, float value, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanRectTransformAnchorMax_y.Register(target, value, duration, ease);
            return target;
        }
    }
}