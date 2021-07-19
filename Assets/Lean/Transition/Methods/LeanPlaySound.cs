﻿using System;
using System.Collections.Generic;
using Lean.Transition.Method;
using UnityEngine;

namespace Lean.Transition.Method
{
    /// <summary>This component allows you to play a sound after the specified duration.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanPlaySound")]
    [AddComponentMenu(
        LeanTransition.MethodsMenuPrefix + "Play Sound" + LeanTransition.MethodsMenuSuffix + "(LeanPlaySound)")]
    public class LeanPlaySound : LeanMethodWithStateAndTarget
    {
        public State Data;

        public override Type GetTargetType()
        {
            return typeof(AudioClip);
        }

        public override void Register()
        {
            PreviousState = Register(GetAliasedTarget(Data.Target), Data.Duration, Data.Volume);
        }

        public static LeanState Register(AudioClip target, float duration, float volume = 1.0f)
        {
            State state = LeanTransition.SpawnWithTarget(State.Pool, target);

            state.Volume = volume;

            return LeanTransition.Register(state, duration);
        }

        [Serializable]
        public class State : LeanStateWithTarget<AudioClip>
        {
            public static Stack<State> Pool = new Stack<State>();

            [Range(0.0f, 1.0f)] public float Volume = 1.0f;

            public override int CanFill => 0;

            public override void UpdateWithTarget(float progress)
            {
                if (progress == 1.0f)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                        return;
#endif
                    GameObject gameObject = new GameObject(Target.name);
                    AudioSource audioSource = gameObject.AddComponent<AudioSource>();

                    audioSource.clip = Target;
                    audioSource.volume = Volume;

                    audioSource.Play();

                    Destroy(gameObject, Target.length);
                }
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
        public static T PlaySoundTransition<T>(this T target, AudioClip clip, float duration = 0.0f,
            float volume = 1.0f)
            where T : Component
        {
            LeanPlaySound.Register(clip, duration, volume);
            return target;
        }

        public static GameObject PlaySoundTransition(this GameObject target, AudioClip clip, float duration = 0.0f,
            float volume = 1.0f)
        {
            LeanPlaySound.Register(clip, duration, volume);
            return target;
        }
    }
}