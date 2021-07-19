using UnityEditor;
using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>
	///     This component rotates the current GameObject based on the current Angle value.
	///     NOTE: This component overrides and takes over the rotation of this GameObject, so you can no longer externally
	///     influence it.
	/// </summary>
	[ExecuteInEditMode]
    [HelpURL(LeanHelper.HelpUrlPrefix + "LeanRoll")]
    [AddComponentMenu(LeanHelper.ComponentPathPrefix + "Roll")]
    public class LeanRoll : MonoBehaviour
    {
        [FSA("Angle")] [SerializeField] private float angle;
        [FSA("Clamp")] [SerializeField] private bool clamp;
        [FSA("ClampMin")] [SerializeField] private float clampMin;
        [FSA("ClampMax")] [SerializeField] private float clampMax;
        [FSA("Damping")] [FSA("Dampening")] [SerializeField] private float damping = -1.0f;

        [SerializeField] private float currentAngle;

        /// <summary>The current angle in degrees.</summary>
        public float Angle
        {
            set => angle = value;
            get => angle;
        }

        /// <summary>Should the <b>Angle</b> value be clamped?</summary>
        public bool Clamp
        {
            set => clamp = value;
            get => clamp;
        }

        /// <summary>The minimum <b>Angle</b> value.</summary>
        public float ClampMin
        {
            set => clampMin = value;
            get => clampMin;
        }

        /// <summary>The maximum <b>Angle</b> value.</summary>
        public float ClampMax
        {
            set => clampMax = value;
            get => clampMax;
        }

		/// <summary>
		///     If you want this component to change smoothly over time, then this allows you to control how quick the changes
		///     reach their target value.
		///     -1 = Instantly change.
		///     1 = Slowly change.
		///     10 = Quickly change.
		/// </summary>
		public float Damping
        {
            set => damping = value;
            get => damping;
        }

        protected virtual void Start()
        {
            currentAngle = angle;
        }

        protected virtual void Update()
        {
            // Get t value
            float factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

            if (clamp)
                angle = Mathf.Clamp(angle, clampMin, clampMax);

            // Lerp angle
            currentAngle = Mathf.LerpAngle(currentAngle, angle, factor);

            // Update rotation
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -currentAngle);
        }

        /// <summary>The <b>Angle</b> value will be incremented by the specified angle in degrees.</summary>
        public void IncrementAngle(float delta)
        {
            angle += delta;
        }

        /// <summary>The <b>Angle</b> value will be decremented by the specified angle in degrees.</summary>
        public void DecrementAngle(float delta)
        {
            angle -= delta;
        }

        /// <summary>This method will update the Angle value based on the specified vector.</summary>
        public void RotateToDelta(Vector2 delta)
        {
            if (delta.sqrMagnitude > 0.0f)
                angle = Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg;
        }

        /// <summary>This method will immediately snap the current angle to its target value.</summary>
        [ContextMenu("Snap To Target")]
        public void SnapToTarget()
        {
            currentAngle = angle;
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Common.Editor
{
    using TARGET = LeanRoll;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanRoll_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("angle", "The current angle in degrees.");
            Draw("clamp", "Should the Angle value be clamped?");

            if (Any(tgts, t => t.Clamp))
            {
                BeginIndent();
                Draw("clampMin", "The minimum Angle value.", "Min");
                Draw("clampMax", "The maximum Angle value.", "Max");
                EndIndent();

                Separator();
            }

            Draw("damping",
                "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
        }
    }
}

#endif