using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;
using UnityEngine.Serialization;
using TARGET = UnityEngine.AudioSource;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to transition the AudioSource's pitch value.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanAudioSourcePitch")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix +
        "AudioSource/AudioSource.pitch" +
        LeanTransition.MethodsMenuSuffix +
        "(LeanAudioSourcePitch)")]
    public class LeanAudioSourcePitch : LeanMethodWithStateAndTarget
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

            [Tooltip("The pitch value will transition to this.")] [FormerlySerializedAs("Pitch")] [Range(-3.0f, 3.0f)] public float Value = 1.0f;

            [Tooltip("This allows you to control how the transition will look.")] public LeanEase Ease =
                LeanEase.Smooth;

            [NonSerialized] private float oldValue;

            public override int CanFill => Target != null && Target.pitch != Value ? 1 : 0;

            public override void FillWithTarget()
            {
                Value = Target.pitch;
            }

            public override void BeginWithTarget()
            {
                oldValue = Target.pitch;
            }

            public override void UpdateWithTarget(float progress)
            {
                Target.pitch = Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
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
        public static TARGET pitchTransition(this TARGET target, float value, float duration,
            LeanEase ease = LeanEase.Smooth)
        {
            LeanAudioSourcePitch.Register(target, value, duration, ease);
            return target;
        }
    }
}