using System;
using Lean.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lean.Gui
{
    /// <summary>This component allows you to change a UI element's hitbox to use its graphic Image opacity/alpha.</summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanHitbox")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Hitbox")]
    public class LeanHitbox : MonoBehaviour
    {
        [SerializeField] private float threshold = 0.5f;

        [NonSerialized] private Image cachedImage;

        [NonSerialized] private bool cachedImageSet;

	    /// <summary>
	    ///     The alpha threshold specifies the minimum alpha a pixel must have for the event to be considered a "hit" on
	    ///     the Image.
	    /// </summary>
	    public float Threshold
        {
            set
            {
                threshold = value;
                UpdateThreshold();
            }
            get => threshold;
        }

        public Image CachedImage
        {
            get
            {
                if (cachedImageSet == false)
                {
                    cachedImage = GetComponent<Image>();
                    cachedImageSet = true;
                }

                return cachedImage;
            }
        }

        protected virtual void Start()
        {
            UpdateThreshold();
        }

        public void UpdateThreshold()
        {
            CachedImage.alphaHitTestMinimumThreshold = threshold;
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanHitbox;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanHitbox_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            if (Draw("threshold",
                "The alpha threshold specifies the minimum alpha a pixel must have for the event to be considered a 'hit' on the Image."))
                Each(tgts, t => t.UpdateThreshold(), true);
        }
    }
}

#endif