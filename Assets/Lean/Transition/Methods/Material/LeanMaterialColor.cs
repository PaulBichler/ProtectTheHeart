﻿using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to transition the specified <b>Material</b>'s <b>color</b> to the target value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanMaterialColor")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "Material/Material color" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanMaterialColor)")]
    public class LeanMaterialColor : LeanMethodWithStateAndTarget
    {
        public State Data;

        public override Type GetTargetType()
        {
            return typeof(Material);
        }

        public override void Register()
        {
            PreviousState = Register(GetAliasedTarget(Data.Target), Data.Property, Data.Color, Data.Duration, Data.Ease);
        }

        public static LeanState Register(Material target, string property, Color color, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            State state = LeanTransition.SpawnWithTarget(State.Pool, target);

            state.Property = property;
            state.Color = color;
            state.Ease = ease;

            return LeanTransition.Register(state, duration);
        }

        [Serializable]
        public class State : LeanStateWithTarget<Material>
        {
            public static Stack<State> Pool = new Stack<State>();

            [Tooltip("The name of the color property in the shader.")] public string Property = "_Color";

            [Tooltip("The color we will transition to.")] public Color Color = Color.white;

            [Tooltip("The ease method that will be used for the transition.")] public LeanEase Ease = LeanEase.Smooth;

            [NonSerialized] private Color oldColor;

            public override int CanFill => Target != null && Target.GetColor(Property) != Color ? 1 : 0;

            public override void FillWithTarget()
            {
                Color = Target.GetColor(Property);
            }

            public override void BeginWithTarget()
            {
                oldColor = Target.GetColor(Property);
            }

            public override void UpdateWithTarget(float progress)
            {
                Target.SetColor(Property, Color.LerpUnclamped(oldColor, Color, Smooth(Ease, progress)));
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
        public static Material colorTransition(this Material target, string property, Color color, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanMaterialColor.Register(target, property, color, duration, ease);
            return target;
        }
    }
}