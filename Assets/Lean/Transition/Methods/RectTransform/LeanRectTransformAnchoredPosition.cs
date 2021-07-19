using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;
using UnityEngine.Serialization;
using TARGET = UnityEngine.RectTransform;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to transition the RectTransform's anchoredPosition value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanRectTransformAnchoredPosition")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "RectTransform/RectTransform.anchoredPosition" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanRectTransformAnchoredPosition)")]
    public class LeanRectTransformAnchoredPosition : LeanMethodWithStateAndTarget
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

        public static LeanState Register(TARGET target, Vector2 value, float duration, LeanEase ease = LeanEase.Smooth)
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

            [Tooltip("The anchoredPosition value will transition to this.")] [FormerlySerializedAs("Position")] public
                Vector2 Value;

            [Tooltip("This allows you to control how the transition will look.")] public LeanEase Ease =
                LeanEase.Smooth;

            [NonSerialized] private Vector2 oldValue;

            public override int CanFill => Target != null && Target.anchoredPosition != Value ? 1 : 0;

            public override void FillWithTarget()
            {
                Value = Target.anchoredPosition;
            }

            public override void BeginWithTarget()
            {
                oldValue = Target.anchoredPosition;
            }

            public override void UpdateWithTarget(float progress)
            {
                Target.anchoredPosition = Vector2.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
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
        public static TARGET anchoredPositionTransition(this TARGET target, Vector2 value, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanRectTransformAnchoredPosition.Register(target, value, duration, ease);
            return target;
        }
    }
}