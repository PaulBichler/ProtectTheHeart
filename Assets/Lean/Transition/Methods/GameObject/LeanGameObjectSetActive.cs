﻿using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;

namespace Lean.Transition.Method
{
	/// <summary>
	///     This component will call <b>GameObject.SetActive</b> with the specified <b>Active</b> state when this
	///     transition completes.
	/// </summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanGameObjectSetActive")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "GameObject/GameObject.SetActive" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanGameObjectSetActive)")]
    public class LeanGameObjectSetActive : LeanMethodWithStateAndTarget
    {
        public State Data;

        public override Type GetTargetType()
        {
            return typeof(GameObject);
        }

        public override void Register()
        {
            PreviousState = Register(GetAliasedTarget(Data.Target), Data.Active, Data.Duration);
        }

        public static LeanState Register(GameObject target, bool active, float duration)
        {
            State state = LeanTransition.SpawnWithTarget(State.Pool, target);

            state.Active = active;

            return LeanTransition.Register(state, duration);
        }

        [Serializable]
        public class State : LeanStateWithTarget<GameObject>
        {
            public static Stack<State> Pool = new Stack<State>();

            [Tooltip("The state we will transition to.")] public bool Active;

            public override int CanFill => Target != null && Target.activeSelf != Active ? 1 : 0;

            public override void FillWithTarget()
            {
                Active = Target.activeSelf;
            }

            public override void UpdateWithTarget(float progress)
            {
                if (progress == 1.0f)
                    Target.SetActive(Active);
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
        public static GameObject SetActiveTransition(this GameObject target, bool active, float duration)
        {
            LeanGameObjectSetActive.Register(target, active, duration);
            return target;
        }
    }
}