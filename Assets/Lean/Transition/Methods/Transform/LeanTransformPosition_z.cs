using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;
using UnityEngine.Serialization;
using TARGET = UnityEngine.Transform;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to transition the Transform's position.z value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformPosition_Z")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "Transform/Transform.position.z" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanTransformPosition_Z)")]
    public class LeanTransformPosition_Z : LeanMethodWithStateAndTarget
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

            [Tooltip("The position value will transition to this.")] [FormerlySerializedAs("Position")] public float
                Value;

            [Tooltip("This allows you to control how the transition will look.")] public LeanEase Ease =
                LeanEase.Smooth;

            [NonSerialized] private float oldValue;

            public override int CanFill => Target != null && Target.position.z != Value ? 1 : 0;

            public override void FillWithTarget()
            {
                Value = Target.position.z;
            }

            public override void BeginWithTarget()
            {
                oldValue = Target.position.z;
            }

            public override void UpdateWithTarget(float progress)
            {
                Vector3 vector = Target.position;

                vector.z = Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));

                Target.position = vector;
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
        public static TARGET positionTransition_z(this TARGET target, float value, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanTransformPosition_Z.Register(target, value, duration, ease);
            return target;
        }
    }
}