using Lean.Common;
using UnityEditor;
using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Gui
{
	/// <summary>
	///     This component allows you to shake the specified Transform, with various controls for shake axes, strength,
	///     dampening, etc.
	///     NOTE: This component works with normal Transforms as well as UI RectTranforms.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanShake")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Shake")]
    public class LeanShake : MonoBehaviour
    {
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private float strength = 20.0f;
        [SerializeField] private float multiplier = 1.0f;
        [FSA("Dampening")] [SerializeField] private float damping;
        [SerializeField] private float reduction;
        [SerializeField] private Vector3 shakePosition = new Vector3(1.0f, 1.0f, 0.0f);
        [SerializeField] private Vector3 shakeRotation = new Vector3(0.0f, 0.0f, 1.0f);

        [SerializeField] private Vector3 offsetPosition;

        [SerializeField] private Vector3 offsetRotation;

        [SerializeField] private Vector3 localPosition;

        [SerializeField] private Quaternion localRotation;

        [SerializeField] private Vector3 expectedPosition;

        [SerializeField] private Quaternion expectedRotation;

        /// <summary>This allows you to set the speed of the shake animation.</summary>
        public float Speed
        {
            set => speed = value;
            get => speed;
        }

		/// <summary>
		///     This allows you to set the current strength of the shake. This value can automatically decay based on the
		///     <b>Dampening</b> and <b>Reduction</b> settings.
		/// </summary>
		public float Strength
        {
            set => strength = value;
            get => strength;
        }

        /// <summary>This allows you to set the final shake strength multiplier. This remains constant.</summary>
        public float Multiplier
        {
            set => multiplier = value;
            get => multiplier;
        }

        /// <summary>This allows you to set the dampening of the <b>Strength</b> value. This decay slows down as it approaches 0.</summary>
        public float Damping
        {
            set => damping = value;
            get => damping;
        }

		/// <summary>
		///     This allows you to set the reduction of the <b>Strength</b> value. This decay slows down at a constant rate
		///     per second.
		/// </summary>
		public float Reduction
        {
            set => reduction = value;
            get => reduction;
        }

        /// <summary>This allows you to set the position axes you want to shake in local units.</summary>
        public Vector3 ShakePosition
        {
            set => shakePosition = value;
            get => shakePosition;
        }

        /// <summary>This allows you to set the rotation axes you want to shake in degrees.</summary>
        public Vector3 ShakeRotation
        {
            set => shakeRotation = value;
            get => shakeRotation;
        }

        protected virtual void Start()
        {
            offsetPosition.x = Random.Range(0.0f, 2.0f);
            offsetPosition.y = Random.Range(0.0f, 2.0f);
            offsetPosition.z = Random.Range(0.0f, 2.0f);

            offsetRotation.x = Random.Range(0.0f, 2.0f);
            offsetRotation.y = Random.Range(0.0f, 2.0f);
            offsetRotation.z = Random.Range(0.0f, 2.0f);

            Transform finalTransform = transform;
            RectTransform finalRectTransform = transform as RectTransform;

            localRotation = expectedRotation = finalTransform.localRotation;

            if (finalRectTransform != null)
                localPosition = expectedPosition = finalRectTransform.anchoredPosition3D;
            else
                localPosition = expectedPosition = finalTransform.localPosition;
        }

        protected virtual void Update()
        {
            float factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);
            Vector3 position = default(Vector3);
            Vector3 rotation = default(Vector3);

            strength = Mathf.Lerp(strength, 0.0f, factor);
            strength = Mathf.MoveTowards(strength, 0.0f, reduction * Time.deltaTime);

            position.x = Sample(ref offsetPosition.x, 29.0f) * shakePosition.x;
            position.y = Sample(ref offsetPosition.y, 31.0f) * shakePosition.y;
            position.z = Sample(ref offsetPosition.z, 37.0f) * shakePosition.z;

            rotation.x = Sample(ref offsetRotation.x, 41.0f) * shakeRotation.x;
            rotation.y = Sample(ref offsetRotation.y, 43.0f) * shakeRotation.y;
            rotation.z = Sample(ref offsetRotation.z, 47.9f) * shakeRotation.z;

            Transform finalTransform = transform;
            RectTransform finalRectTransform = transform as RectTransform;

            // Rotation
            Quaternion currentRotation = finalTransform.localRotation;

            if (currentRotation != expectedRotation)
                localRotation = currentRotation;

            finalTransform.localRotation =
                expectedRotation = localRotation * Quaternion.Euler(rotation * strength * multiplier);

            // Position
            if (finalRectTransform != null)
            {
                Vector3 currentPosition = finalRectTransform.anchoredPosition3D;

                if (currentPosition != expectedPosition)
                    localPosition = currentPosition;

                finalRectTransform.anchoredPosition3D =
                    expectedPosition = localPosition + position * strength * multiplier;
            }
            else
            {
                Vector3 currentPosition = finalTransform.localPosition;

                if (currentPosition != expectedPosition)
                    localPosition = currentPosition;

                finalTransform.localPosition = expectedPosition = localPosition + position * strength * multiplier;
            }
        }

        public void Shake(float addedStrength)
        {
            strength += addedStrength;
        }

        private float Sample(ref float x, float prime)
        {
            x = (x + speed * prime * Time.deltaTime) % 2.0f;

            return Mathf.SmoothStep(-1.0f, 1.0f, x > 1.0f ? 2.0f - x : x);
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanShake;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanShake_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("speed", "This allows you to set the speed of the shake animation.");
            Draw("strength",
                "This allows you to set the current strength of the shake. This value can automatically decay based on the Dampening and Reduction settings.");
            Draw("multiplier", "This allows you to set the final shake strength multiplier. This remains constant.");
            Draw("damping",
                "This allows you to set the dampening of the Strength value. This decay slows down as it approaches 0.");
            Draw("reduction",
                "This allows you to set the reduction of the Strength value. This decay slows down at a constant rate per second.");

            Separator();

            Draw("shakePosition", "This allows you to set the position axes you want to shake in local units.");
            Draw("shakeRotation", "This allows you to set the rotation axes you want to shake in degrees.");
        }
    }
}

#endif