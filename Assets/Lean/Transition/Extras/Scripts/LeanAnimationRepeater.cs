using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Transition
{
    /// <summary>This component executes the specified transitions at regular intervals.</summary>
    [HelpURL(LeanTransition.HelpUrlPrefix + "LeanAnimationRepeater")]
    [AddComponentMenu(LeanTransition.ComponentMenuPrefix + "Lean Animation Repeater")]
    public class LeanAnimationRepeater : LeanManualAnimation
    {
        [SerializeField] protected int remainingCount = -1;
        [SerializeField] [FSA("RemainingTime")] protected float remainingTime = 1.0f;
        [SerializeField] [FSA("TimeInterval")] private float timeInterval = 3.0f;
        [SerializeField] [FSA("OnAnimation")] protected UnityEvent onAnimation;

	    /// <summary>
	    ///     The amount of times this component can begin the specified transitions.
	    ///     -1 = Unlimited.
	    /// </summary>
	    public int RemainingCount
        {
            set => remainingCount = value;
            get => remainingCount;
        }

        /// <summary>When this reaches 0, the transitions will begin.</summary>
        public float RemainingTime
        {
            set => remainingTime = value;
            get => remainingTime;
        }

        /// <summary>When <b>RemainingTime</b> reaches 0, it will bet set to this value.</summary>
        public float TimeInterval
        {
            set => timeInterval = value;
            get => timeInterval;
        }

        /// <summary>The event will execute when the transitions begin.</summary>
        public UnityEvent OnAnimation
        {
            get
            {
                if (onAnimation == null) onAnimation = new UnityEvent();
                return onAnimation;
            }
        }

        protected virtual void Start()
        {
            if (remainingTime <= 0.0f)
                TryBegin();
        }

        protected virtual void Update()
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0.0f)
                TryBegin();
        }

        private void TryBegin()
        {
            remainingTime = timeInterval + remainingTime % timeInterval;

            if (remainingCount >= 0)
            {
                if (remainingCount == 0)
                    return;

                remainingCount -= 1;
            }

            BeginTransitions();

            if (onAnimation != null)
                onAnimation.Invoke();
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Transition.Editor
{
    using TARGET = LeanAnimationRepeater;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanAnimationRepeater_Editor : LeanManualAnimation_Editor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("remainingCount",
                "The amount of times this component can begin the specified transitions.\n\n-1 = Unlimited.");
            Draw("remainingTime", "When this reaches 0, the transitions will begin.");
            Draw("timeInterval", "When <b>RemainingTime</b> reaches 0, it will bet set to this value.");

            Separator();

            base.OnInspector();

            Separator();

            Draw("onAnimation");
        }
    }
}

#endif