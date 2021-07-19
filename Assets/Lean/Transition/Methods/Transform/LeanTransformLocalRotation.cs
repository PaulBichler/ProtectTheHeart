﻿using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to transition the specified Transform.localRotation to the target value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformLocalRotation")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "Transform/Transform.localRotation" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanTransformLocalRotation)")]
    public class LeanTransformLocalRotation : LeanMethodWithStateAndTarget
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

        public static LeanState Register(Transform target, Quaternion rotation, float duration,
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

            [Tooltip("The rotation we will transition to.")] public Quaternion Rotation = Quaternion.identity;

            [Tooltip("The ease method that will be used for the transition.")] public LeanEase Ease = LeanEase.Smooth;

            [NonSerialized] private Quaternion oldRotation;

            public override void FillWithTarget()
            {
                Rotation = Target.localRotation;
            }

            public override void BeginWithTarget()
            {
                oldRotation = Target.localRotation;
            }

            public override void UpdateWithTarget(float progress)
            {
                Target.localRotation = Quaternion.SlerpUnclamped(oldRotation, Rotation, Smooth(Ease, progress));
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
        public static Transform localRotationTransition(this Transform target, Quaternion rotation, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanTransformLocalRotation.Register(target, rotation, duration, ease);
            return target;
        }
    }
}