﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lean.Transition.Method
{
    /// <summary>This component calls the <b>RectTransform.SetAsLastSibling</b> method when this transition completes.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanRectTransformSetAsLastSibling")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "RectTransform/RectTransform.SetAsLastSibling" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanRectTransformSetAsLastSibling)")]
    public class LeanRectTransformSetAsLastSibling : LeanMethodWithStateAndTarget
    {
        public State Data;

        public override Type GetTargetType()
        {
            return typeof(RectTransform);
        }

        public override void Register()
        {
            PreviousState = Register(GetAliasedTarget(Data.Target), Data.Duration);
        }

        public static LeanState Register(RectTransform target, float duration)
        {
            State state = LeanTransition.SpawnWithTarget(State.Pool, target);

            return LeanTransition.Register(state, duration);
        }

        [Serializable]
        public class State : LeanStateWithTarget<RectTransform>
        {
            public static Stack<State> Pool = new Stack<State>();

            public override void UpdateWithTarget(float progress)
            {
                if (progress == 1.0f)
                    Target.SetAsLastSibling();
            }

            public override void Despawn()
            {
                Pool.Push(this);
            }
        }
    }
}