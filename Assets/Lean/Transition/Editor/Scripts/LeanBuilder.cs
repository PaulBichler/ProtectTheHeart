using System;
using System.Collections.Generic;
using System.IO;
using Lean.Common;
using UnityEditor;
using UnityEngine;

namespace Lean.Transition.Editor
{
    /// <summary>This allows you to build a transition method from the specified template.</summary>
    [CreateAssetMenu(fileName = "NewBuilder", menuName = "Lean/Transition/Builder")]
    public class LeanBuilder : ScriptableObject
    {
        [SerializeField] private List<Entry> entries;

        /// <summary>This allows you to specify which transition methods will be built.</summary>
        public List<Entry> Entries
        {
            get
            {
                if (entries == null) entries = new List<Entry>();
                return entries;
            }
        }

        public void Build()
        {
            if (entries != null)
            {
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));

                string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
                string longName = name;
                string shortName = name;
                int index = shortName.LastIndexOf(".");

                if (index >= 0)
                    shortName = shortName.Substring(index + 1);

                foreach (Entry entry in entries)
                    Build(entry, longName, shortName, path + "/Lean" + shortName + entry.Title + ".cs");
            }
        }

        private void Build(Entry entry, string longName, string shortName, string path)
        {
            string body = entry.Template.Body;

            body = body.Replace("{COMPONENT_FULL}", longName);
            body = body.Replace("{COMPONENT}", shortName);
            body = body.Replace("{TITLE}", shortName + entry.Title);

            string[] contents = entry.Contents.Split(new[] {
                '\r', '\n'
            }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string content in contents)
            {
                Vector2Int indices = GetIndex(content);
                string left = content.Substring(0, indices.x);
                string right = content.Substring(indices.y);

                body = body.Replace("{" + left + "}", right);
            }

            body = body.Replace("{ATTRIBUTE}", "");
            body = body.Replace("{FSA}", "");
            body = body.Replace("{DEFAULT}", "");

            File.WriteAllText(path, body);

            AssetDatabase.ImportAsset(path);
        }

        private static Vector2Int GetIndex(string s)
        {
            int a = s.IndexOf('\t');

            for (int i = a + 1; i < s.Length; i++)
                if (s[i] != '	')
                    return new Vector2Int(a, i);

            return new Vector2Int(-1, -1);
        }

        [Serializable]
        public class Entry
        {
            public string Title;
            public LeanTemplate Template;

            [Multiline(8)] public string Contents;
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(LeanBuilder))]
    public class LeanBuilder_Editor : LeanEditor
    {
        protected override void OnInspector()
        {
            LeanBuilder tgt;
            LeanBuilder[] tgts;
            GetTargets(out tgt, out tgts);

            EditorGUILayout.LabelField("Transition Builder", EditorStyles.boldLabel);

            Separator();

            BeginLabelWidth(100);
            Draw("entries", "This allows you to specify which transition methods will be built.");
            EndLabelWidth();

            Separator();

            if (GUILayout.Button("BUILD"))
                Each(tgts, t => t.Build());
        }
    }
}