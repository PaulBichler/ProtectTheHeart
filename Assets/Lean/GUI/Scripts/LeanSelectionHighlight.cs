using System;
using Lean.Common;
using Lean.Transition;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Lean.Gui
{
    /// <summary>This component will place the current RectTransform above the currently selected object.</summary>
    [RequireComponent(typeof(RectTransform))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanSelectionHighlight")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Selection Highlight")]
    public class LeanSelectionHighlight : MonoBehaviour
    {
        [SerializeField] private Camera worldCamera;
        [SerializeField] private LeanPlayer showTransitions;
        [SerializeField] private LeanPlayer hideTransitions;
        [SerializeField] private UnityEvent onShow;
        [SerializeField] private UnityEvent onHide;

        [SerializeField] private bool showing;

        [NonSerialized] protected RectTransform canvasRectTransform;

        [NonSerialized] protected RectTransform rectTransform;

	    /// <summary>
	    ///     The camera rendering the target transform/position.
	    ///     None = MainCamera.
	    /// </summary>
	    public Camera WorldCamera
        {
            set => worldCamera = value;
            get => worldCamera;
        }

	    /// <summary>
	    ///     This allows you to perform a transition when the highlight begins.
	    ///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
	    ///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
	    ///     NOTE: Any transitions you perform here must be reverted in the <b>Hide Transitions</b> setting using a matching
	    ///     transition component.
	    /// </summary>
	    public LeanPlayer ShowTransitions
        {
            get
            {
                if (showTransitions == null) showTransitions = new LeanPlayer();
                return showTransitions;
            }
        }

	    /// <summary>
	    ///     This allows you to perform a transition when the highlight ends.
	    ///     You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
	    ///     For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
	    ///     NOTE: Any transitions you perform here must be reverted in the <b>Show Transitions</b> setting using a matching
	    ///     transition component.
	    /// </summary>
	    public LeanPlayer HideTransitions
        {
            get
            {
                if (hideTransitions == null) hideTransitions = new LeanPlayer();
                return hideTransitions;
            }
        }

        /// <summary>This allows you to perform an action when the highlight starts.</summary>
        public UnityEvent OnShow
        {
            get
            {
                if (onShow == null) onShow = new UnityEvent();
                return onShow;
            }
        }

        /// <summary>This allows you to perform an action when the highlight ends.</summary>
        public UnityEvent OnHide
        {
            get
            {
                if (onHide == null) onHide = new UnityEvent();
                return onHide;
            }
        }

        protected virtual void LateUpdate()
        {
            EventSystem eventSystem = EventSystem.current;
            bool show = false;

            if (eventSystem != null)
            {
                GameObject selected = eventSystem.currentSelectedGameObject;

                if (selected != null)
                {
                    RectTransform selectedRect = selected.GetComponent<RectTransform>();

                    if (selectedRect != null)
                        show = UpdateRect(selectedRect);
                }
            }

            if (showing != show)
            {
                showing = show;

                if (showing)
                {
                    if (showTransitions != null)
                        showTransitions.Begin();
                }
                else
                {
                    if (hideTransitions != null)
                        hideTransitions.Begin();
                }
            }
        }

        private bool UpdateRect(RectTransform target)
        {
            Camera camera = worldCamera;

            if (camera == null)
                camera = Camera.main;

            if (camera != null)
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();

                if (canvasRectTransform == null)
                {
                    Canvas canvas = GetComponentInParent<Canvas>();

                    if (canvas == null)
                        throw new Exception("Couldn't find attached canvas??");

                    canvasRectTransform = canvas.GetComponent<RectTransform>();
                }

                // Calculate viewport/anchor points
                Vector2 min = target.rect.min;
                Vector2 max = target.rect.max;
                Vector3 targetA = target.TransformPoint(min.x, min.y, 0.0f);
                Vector3 targetB = target.TransformPoint(max.x, min.y, 0.0f);
                Vector3 targetC = target.TransformPoint(min.x, max.y, 0.0f);
                Vector3 targetD = target.TransformPoint(max.x, max.y, 0.0f);

                bool worldSpace = target.GetComponentInParent<Canvas>().renderMode == RenderMode.WorldSpace;
                Vector3 viewportPointA = WorldToViewportPoint(camera, targetA, worldSpace);
                Vector3 viewportPointB = WorldToViewportPoint(camera, targetB, worldSpace);
                Vector3 viewportPointC = WorldToViewportPoint(camera, targetC, worldSpace);
                Vector3 viewportPointD = WorldToViewportPoint(camera, targetD, worldSpace);

                // If outside frustum, hide line out of view
                if (LeanGui.InvaidViewportPoint(camera, viewportPointA) ||
                    LeanGui.InvaidViewportPoint(camera, viewportPointB) ||
                    LeanGui.InvaidViewportPoint(camera, viewportPointC) ||
                    LeanGui.InvaidViewportPoint(camera, viewportPointD))
                    viewportPointA = viewportPointB = viewportPointC = viewportPointD = new Vector3(10.0f, 10.0f);

                float minX = Mathf.Min(Mathf.Min(viewportPointA.x, viewportPointB.x),
                    Mathf.Min(viewportPointC.x, viewportPointD.x));
                float minY = Mathf.Min(Mathf.Min(viewportPointA.y, viewportPointB.y),
                    Mathf.Min(viewportPointC.y, viewportPointD.y));
                float maxX = Mathf.Max(Mathf.Max(viewportPointA.x, viewportPointB.x),
                    Mathf.Max(viewportPointC.x, viewportPointD.x));
                float maxY = Mathf.Max(Mathf.Max(viewportPointA.y, viewportPointB.y),
                    Mathf.Max(viewportPointC.y, viewportPointD.y));

                // Convert viewport points to canvas points
                Rect canvasRect = canvasRectTransform.rect;
                float canvasXA = canvasRect.xMin + canvasRect.width * minX;
                float canvasYA = canvasRect.yMin + canvasRect.height * minY;
                float canvasXB = canvasRect.xMin + canvasRect.width * maxX;
                float canvasYB = canvasRect.yMin + canvasRect.height * maxY;

                // Find center, reset anchor, and convert canvas point to world point
                float canvasX = (canvasXA + canvasXB) * 0.5f;
                float canvasY = (canvasYA + canvasYB) * 0.5f;

                rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
                rectTransform.sizeDelta = new Vector2(canvasXB - canvasXA, canvasYB - canvasYA);
                rectTransform.position = canvasRectTransform.TransformPoint(canvasX, canvasY, 0.0f);

                // Get vector between points

                return true;
            }

            return false;
        }

        private static Vector3 WorldToViewportPoint(Camera camera, Vector3 point, bool worldSpace)
        {
            if (worldSpace == false)
            {
                point = RectTransformUtility.WorldToScreenPoint(null, point);
                point.z = 0.5f;

                return camera.ScreenToViewportPoint(point);
            }

            return camera.WorldToViewportPoint(point);
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Gui.Editor
{
    using TARGET = LeanSelectionHighlight;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanSelectionHighlight_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("worldCamera", "The camera rendering the target transform/position.\nNone = MainCamera.");

            Separator();

            Draw("showTransitions",
                "This allows you to perform a transition when the highlight begins. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here must be reverted in the Hide Transitions setting using a matching transition component.");
            Draw("hideTransitions",
                "This allows you to perform a transition when the highlight ends. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here must be reverted in the Show Transitions setting using a matching transition component.");

            Separator();

            Draw("onShow", "This allows you to perform an action when the highlight starts.");
            Draw("onHide", "This allows you to perform an action when the highlight ends.");
        }
    }
}

#endif