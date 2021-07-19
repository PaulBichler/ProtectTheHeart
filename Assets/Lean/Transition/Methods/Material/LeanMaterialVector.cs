﻿using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to transition the specified <b>Material</b>'s <b>vector</b> to the target value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanMaterialVector")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "Material/Material vector" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanMaterialVector)")]
    public class LeanMaterialVector : LeanMethodWithStateAndTarget
    {
        public State Data;

        public override Type GetTargetType()
        {
            return typeof(Material);
        }

        public override void Register()
        {
            PreviousState = Register(GetAliasedTarget(Data.Target), Data.Property, Data.Value, Data.Duration, Data.Ease);
        }

        public static LeanState Register(Material target, string property, Vector4 value, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            State state = LeanTransition.SpawnWithTarget(State.Pool, target);

            state.Property = property;
            state.Value = value;
            state.Ease = ease;

            return LeanTransition.Register(state, duration);
        }

        [Serializable]
        public class State : LeanStateWithTarget<Material>
        {
            public static Stack<State> Pool = new Stack<State>();

            [Tooltip("The name of the vector property in the shader.")] public string Property;

            [Tooltip("The value we will transition to.")] public Vector4 Value;

            [Tooltip("The ease method that will be used for the transition.")] public LeanEase Ease = LeanEase.Smooth;

            [NonSerialized] private Vector4 oldValue;

            public override int CanFill => Target != null && Target.GetVector(Property) != Value ? 1 : 0;

            public override void FillWithTarget()
            {
                Value = Target.GetVector(Property);
            }

            public override void BeginWithTarget()
            {
                oldValue = Target.GetVector(Property);
            }

            public override void UpdateWithTarget(float progress)
            {
                Target.SetVector(Property, Vector4.LerpUnclamped(oldValue, Value, Smooth(Ease, progress)));
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
        public static Material vectorTransition(this Material target, string property, Vector4 value, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanMaterialVector.Register(target, property, value, duration, ease);
            return target;
        }
    }
}