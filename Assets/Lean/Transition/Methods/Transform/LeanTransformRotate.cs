﻿using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to rotate the specified Transform by the target value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformRotate")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "Transform/Transform.Rotate" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanTransformRotate)")]
    public class LeanTransformRotate : LeanMethodWithStateAndTarget
    {
        public State Data;

        public override Type GetTargetType()
        {
            return typeof(Transform);
        }

        public override void Register()
        {
            PreviousState = Register(GetAliasedTarget(Data.Target), Data.EulerAngles, Data.Space, Data.Duration,
                Data.Ease);
        }

        public static LeanState Register(Transform target, Vector3 eulerAngles, Space space, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            State state = LeanTransition.SpawnWithTarget(State.Pool, target);

            state.EulerAngles = eulerAngles;
            state.Space = space;
            state.Ease = ease;

            return LeanTransition.Register(state, duration);
        }

        [Serializable]
        public class State : LeanStateWithTarget<Transform>
        {
            public static Stack<State> Pool = new Stack<State>();

            [Tooltip("The amount we will rotate.")] public Vector3 EulerAngles;

            [Tooltip("The space we will transition in.")] public Space Space = Space.Self;

            [Tooltip("The ease method that will be used for the transition.")] public LeanEase Ease = LeanEase.Smooth;

            [NonSerialized] private Vector3 previousEulerAngles;

            public override ConflictType Conflict => ConflictType.None;

            public override void BeginWithTarget()
            {
                previousEulerAngles = Vector3.zero;
            }

            public override void UpdateWithTarget(float progress)
            {
                Vector3 eulerAngles = EulerAngles * Smooth(Ease, progress);

                Target.Rotate(eulerAngles - previousEulerAngles, Space);

                previousEulerAngles = eulerAngles;
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
        public static Transform RotateTransition(this Transform target, float x, float y, float z, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanTransformRotate.Register(target, new Vector3(x, y, z), Space.Self, duration, ease);
            return target;
        }

        public static Transform RotateTransition(this Transform target, Vector3 eulerAngles, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanTransformRotate.Register(target, eulerAngles, Space.Self, duration, ease);
            return target;
        }

        public static Transform RotateTransition(this Transform target, float x, float y, float z, Space space,
            float duration, LeanEase ease = LeanEase.Smooth)
        {
            LeanTransformRotate.Register(target, new Vector3(x, y, z), space, duration, ease);
            return target;
        }

        public static Transform RotateTransition(this Transform target, Vector3 eulerAngles, Space space,
            float duration, LeanEase ease = LeanEase.Smooth)
        {
            LeanTransformRotate.Register(target, eulerAngles, space, duration, ease);
            return target;
        }
    }
}