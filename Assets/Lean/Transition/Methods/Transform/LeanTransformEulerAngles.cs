﻿using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to transition the specified <b>Transform.eulerAngles</b> to the target value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformEulerAngles")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "Transform/Transform.eulerAngles" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanTransformEulerAngles)")]
    public class LeanTransformEulerAngles : LeanMethodWithStateAndTarget
    {
        public State Data;

        public override Type GetTargetType()
        {
            return typeof(Transform);
        }

        public override void Register()
        {
            PreviousState = Register(GetAliasedTarget(Data.Target), Data.Rotation, Data.Duration, Data.Ease);
        }

        public static LeanState Register(Transform target, Vector3 rotation, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            State state = LeanTransition.SpawnWithTarget(State.Pool, target);

            state.Rotation = rotation;
            state.Ease = ease;

            return LeanTransition.Register(state, duration);
        }

        [Serializable]
        public class State : LeanStateWithTarget<Transform>
        {
            public static Stack<State> Pool = new Stack<State>();

            [Tooltip("The rotation we will transition to.")] public Vector3 Rotation;

            [Tooltip("The ease method that will be used for the transition.")] public LeanEase Ease = LeanEase.Smooth;

            [NonSerialized] private Vector3 oldRotation;

            public override int CanFill => Target != null && Target.eulerAngles != Rotation ? 1 : 0;

            public override void FillWithTarget()
            {
                Rotation = Target.eulerAngles;
            }

            public override void BeginWithTarget()
            {
                oldRotation = Target.eulerAngles;
            }

            public override void UpdateWithTarget(float progress)
            {
                Vector3 rotation = Vector3.LerpUnclamped(oldRotation, Rotation, Smooth(Ease, progress));

                Target.rotation = Quaternion.Euler(rotation);
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
        public static Transform eulerAnglesTransform(this Transform target, Vector3 position, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanTransformEulerAngles.Register(target, position, duration, ease);
            return target;
        }
    }
}