﻿using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to delay for a specified duration.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanDelay")]
    [AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Delay" + LeanTransition.MethodsMenuSuffix + "(LeanDelay)")]
    public class LeanDelay : LeanMethodWithState
    {
        public State Data;

        public override void Register()
        {
            PreviousState = Register(Data.Duration);
        }

        public static LeanState Register(float duration)
        {
            State state = LeanTransition.Spawn(State.Pool);

            return LeanTransition.Register(state, duration);
        }

        [Serializable]
        public class State : LeanState
        {
            public static Stack<State> Pool = new Stack<State>();

            public override void Begin()
            {
                // No state to begin from
            }

            public override void Update(float progress)
            {
                // No state to update
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
        /// <summary>This will pause the animation for the specified amount of seconds.</summary>
        public static T DelayTransition<T>(this T target, float duration)
            where T : Component
        {
            LeanDelay.Register(duration);
            return target;
        }

        /// <summary>This will pause the animation for the specified amount of seconds.</summary>
        public static GameObject DelayTransition(this GameObject target, float duration)
        {
            LeanDelay.Register(duration);
            return target;
        }
    }
}