using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
    /// <summary>This component allows you make the current GameObject selectable.</summary>
    [HelpURL(LeanHelper.HelpUrlPrefix + "LeanSelectable")]
    public class LeanSelectable : MonoBehaviour
    {
        public static LinkedList<LeanSelectable> Instances = new LinkedList<LeanSelectable>();

        protected static List<LeanSelectable> tempSelectables = new List<LeanSelectable>();
        [SerializeField] private bool selfSelected;
        [SerializeField] private UnityEvent onSelected;
        [FSA("OnDeselect")] [SerializeField] private UnityEvent onDeselected;
        [NonSerialized] private LinkedListNode<LeanSelectable> instancesNode;

        public bool SelfSelected
        {
            set
            {
                if (selfSelected != value)
                {
                    selfSelected = value;
                    if (value) InvokeOnSelected(null);
                    else InvokeOnDeslected(null);
                }
            }
            get => selfSelected;
        }

	    /// <summary>
	    ///     This is invoked every time this object is selected.
	    ///     NOTE: This may occur multiple times.
	    /// </summary>
	    public UnityEvent OnSelected
        {
            get
            {
                if (onSelected == null) onSelected = new UnityEvent();
                return onSelected;
            }
        }

	    /// <summary>
	    ///     This is invoked every time this object is deselected.
	    ///     NOTE: This may occur multiple times.
	    /// </summary>
	    public UnityEvent OnDeselected
        {
            get
            {
                if (onDeselected == null) onDeselected = new UnityEvent();
                return onDeselected;
            }
        }

        /// <summary>This will tell you how many <b>LeanSelect</b> components in the scene currently have this object selected.</summary>
        public int SelectedCount
        {
            get
            {
                int count = 0;

                if (selfSelected)
                    count += 1;

                foreach (LeanSelect select in LeanSelect.Instances)
                    if (@select.IsSelected(this))
                        count += 1;

                return count;
            }
        }

	    /// <summary>
	    ///     This will tell you if this object is self selected, or selected by any <b>LeanSelect</b> components in the
	    ///     scene.
	    /// </summary>
	    public bool IsSelected
        {
            get
            {
                if (selfSelected)
                    return true;

                foreach (LeanSelect select in LeanSelect.Instances)
                    if (@select.IsSelected(this))
                        return true;

                return false;
            }
        }

        public static int IsSelectedCount
        {
            get
            {
                int count = 0;

                foreach (LeanSelectable selectable in Instances)
                    if (selectable.IsSelected)
                        count += 1;

                return count;
            }
        }

        protected virtual void OnEnable()
        {
            instancesNode = Instances.AddLast(this);

            if (OnAnyEnabled != null)
                OnAnyEnabled.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            Instances.Remove(instancesNode);
            instancesNode = null;

            if (OnAnyDisabled != null)
                OnAnyDisabled.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            Deselect();
        }

        public static event Action<LeanSelectable> OnAnyEnabled;

        public static event Action<LeanSelectable> OnAnyDisabled;

        public static event Action<LeanSelect, LeanSelectable> OnAnySelected;

        public static event Action<LeanSelect, LeanSelectable> OnAnyDeselected;

        [ContextMenu("Deselect")]
        public void Deselect()
        {
            SelfSelected = false;

            foreach (LeanSelect select in LeanSelect.Instances)
                @select.Deselect(this);
        }

        /// <summary>This deselects all objects in the scene.</summary>
        public static void DeselectAll()
        {
            foreach (LeanSelect select in LeanSelect.Instances)
                @select.DeselectAll();

            foreach (LeanSelectable selectable in Instances)
                selectable.SelfSelected = false;
        }

        public void InvokeOnSelected(LeanSelect select)
        {
            if (onSelected != null)
                onSelected.Invoke();

            if (OnAnySelected != null)
                OnAnySelected.Invoke(@select, this);
        }

        public void InvokeOnDeslected(LeanSelect select)
        {
            if (onDeselected != null)
                onDeselected.Invoke();

            if (OnAnyDeselected != null)
                OnAnyDeselected.Invoke(@select, this);
        }

        [Serializable]
        public class LeanSelectEvent : UnityEvent<LeanSelect>
        {
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Common.Editor
{
    using TARGET = LeanSelectable;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanSelectable_Editor : LeanEditor
    {
        [NonSerialized] private TARGET tgt;
        [NonSerialized] private TARGET[] tgts;

        protected override void OnInspector()
        {
            GetTargets(out tgt, out tgts);

            DrawSelected();

            Separator();

            bool showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

            DrawEvents(showUnusedEvents);
        }

        private void DrawSelected()
        {
            BeginDisabled();
            EditorGUILayout.Toggle(
                new GUIContent("Is Selected",
                    "This will tell you if this object is self selected, or selected by any LeanSelect components in the scene."),
                tgt.IsSelected);
            EndDisabled();
            BeginIndent();
            if (Draw("selfSelected"))
                Each(tgts, t => t.SelfSelected = serializedObject.FindProperty("selfSelected").boolValue, true);
            BeginDisabled();
            foreach (LeanSelect select in LeanSelect.Instances)
                if (IsSelectedByAnyTgt(@select))
                    EditorGUILayout.ObjectField(new GUIContent("selectedBy"), @select, typeof(LeanSelect), true);
            EndDisabled();
            EndIndent();
        }

        private bool IsSelectedByAnyTgt(LeanSelect select)
        {
            foreach (LeanSelectable tgt in tgts)
                if (@select.IsSelected(tgt))
                    return true;

            return false;
        }

        protected virtual void DrawEvents(bool showUnusedEvents)
        {
            if (showUnusedEvents || Any(tgts, t => t.OnSelected.GetPersistentEventCount() > 0))
                Draw("onSelected");

            if (showUnusedEvents || Any(tgts, t => t.OnDeselected.GetPersistentEventCount() > 0))
                Draw("onDeselected");
        }
    }
}

#endif