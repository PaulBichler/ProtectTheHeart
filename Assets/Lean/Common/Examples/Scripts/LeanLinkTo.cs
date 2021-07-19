using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Lean.Common.Examples
{
    /// <summary>This component turns the current UI element into a button that will perform a common action you specify.</summary>
    [ExecuteInEditMode]
    [HelpURL(LeanHelper.HelpUrlPrefix + "LeanLinkTo")]
    [AddComponentMenu(LeanHelper.ComponentPathPrefix + "Link To")]
    public class LeanLinkTo : MonoBehaviour, IPointerClickHandler
    {
        public enum LinkType
        {
            Publisher,
            PreviousScene,
            NextScene
        }

        [SerializeField] private LinkType link;

        /// <summary>The action that will be performed when clicked.</summary>
        public LinkType Link
        {
            set => link = value;
            get => link;
        }

        protected virtual void Update()
        {
            switch (link)
            {
                case LinkType.PreviousScene:
                case LinkType.NextScene:
                {
                    CanvasGroup group = GetComponent<CanvasGroup>();

                    if (group != null)
                    {
                        bool show = GetCurrentLevel() >= 0 && GetLevelCount() > 1;

                        group.alpha = show ? 1.0f : 0.0f;
                        group.blocksRaycasts = show;
                        group.interactable = show;
                    }
                }
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (link)
            {
                case LinkType.Publisher:
                {
                    Application.OpenURL("https://carloswilkes.com");
                }
                    break;

                case LinkType.PreviousScene:
                {
                    int index = GetCurrentLevel();

                    if (index >= 0)
                    {
                        if (--index < 0)
                            index = GetLevelCount() - 1;

                        LoadLevel(index);
                    }
                }
                    break;

                case LinkType.NextScene:
                {
                    int index = GetCurrentLevel();

                    if (index >= 0)
                    {
                        if (++index >= GetLevelCount())
                            index = 0;

                        LoadLevel(index);
                    }
                }
                    break;
            }
        }

        private static int GetCurrentLevel()
        {
            Scene scene = SceneManager.GetActiveScene();
            int index = scene.buildIndex;

            if (index >= 0)
                if (SceneManager.GetSceneByBuildIndex(index).path != scene.path)
                    return -1;

            return index;
        }

        private static int GetLevelCount()
        {
            return SceneManager.sceneCountInBuildSettings;
        }

        private static void LoadLevel(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}

#if UNITY_EDITOR

namespace Lean.Common.Examples.Editor
{
    using TARGET = LeanLinkTo;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanLinkTo_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("link", "The action that will be performed when clicked.");
        }
    }
}

#endif