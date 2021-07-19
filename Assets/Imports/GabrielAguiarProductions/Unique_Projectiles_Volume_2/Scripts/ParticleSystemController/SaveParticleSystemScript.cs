using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif

using UnityEngine;

namespace GAP_ParticleSystemController
{
    public static class SaveParticleSystemScript
    {
        public static void SaveVFX(GameObject prefabVFX, List<ParticleSystemOriginalSettings> psOriginalSettingsList)
        {
#if UNITY_2018_3_OR_NEWER
            string prefabFolderPath = GetPrefabFolder2018_3(prefabVFX);
#else
             var prefabFolderPath = GetPrefabFolder (prefabVFX);
#endif

#if UNITY_EDITOR
            if (!Directory.Exists(prefabFolderPath + "/OriginalSettings"))
            {
                AssetDatabase.CreateFolder(prefabFolderPath, "OriginalSettings");
                Debug.Log("Created folder:  " + prefabFolderPath + "/OriginalSettings");
            }
#endif
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(prefabFolderPath + "/OriginalSettings/" + prefabVFX.name + ".dat",
                FileMode.Create);

            bf.Serialize(stream, psOriginalSettingsList);
            stream.Close();

#if UNITY_2018_3_OR_NEWER
            SaveNestedPrefab(prefabVFX);
#endif

            Debug.Log("Original Settings of '" +
                      prefabVFX.name +
                      "' saved to: " +
                      prefabFolderPath +
                      "/OriginalSettings");
        }

        public static List<ParticleSystemOriginalSettings> LoadVFX(GameObject prefabVFX)
        {
#if UNITY_2018_3_OR_NEWER
            string prefabFolderPath = GetPrefabFolder2018_3(prefabVFX);
#else
            var prefabFolderPath = GetPrefabFolder(prefabVFX);
#endif

            if (File.Exists(prefabFolderPath + "/OriginalSettings/" + prefabVFX.name + ".dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(prefabFolderPath + "/OriginalSettings/" + prefabVFX.name + ".dat",
                    FileMode.Open);

                List<ParticleSystemOriginalSettings> originalSettingsList = new List<ParticleSystemOriginalSettings>();
                originalSettingsList = bf.Deserialize(stream) as List<ParticleSystemOriginalSettings>;

                stream.Close();
                return originalSettingsList;
            }
            Debug.Log("No saved VFX data found");
            return null;
        }

        public static bool CheckExistingFile(GameObject prefabVFX)
        {
#if UNITY_2018_3_OR_NEWER
            string prefabFolderPath = GetPrefabFolder2018_3(prefabVFX);
#else
            var prefabFolderPath = GetPrefabFolder(prefabVFX);
#endif
            if (prefabFolderPath != null)
            {
                if (File.Exists(prefabFolderPath + "/OriginalSettings/" + prefabVFX.name + ".dat"))
                    return true;
                return false;
            }
            return false;
        }

        private static string GetPrefabFolder(GameObject prefabVFX)
        {
#if UNITY_EDITOR
            string prefabPath = AssetDatabase.GetAssetPath(prefabVFX);
            string prefabFolderPath = Path.GetDirectoryName(prefabPath);
            return prefabFolderPath;
#else
            return null;
#endif
        }

#if UNITY_2018_3_OR_NEWER
        private static string GetPrefabFolder2018_3(GameObject prefabVFX)
        {
#if UNITY_EDITOR
            string prefabPath = PrefabStageUtility.GetPrefabStage(prefabVFX).assetPath;
            string prefabFolderPath = Path.GetDirectoryName(prefabPath);
            return prefabFolderPath;
#else
            return null;
#endif
        }
#endif

#if UNITY_2018_3_OR_NEWER
        public static void SaveNestedPrefab(GameObject prefab)
        {
#if UNITY_EDITOR
            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(prefab);
            PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, prefabStage.assetPath);
#endif
        }
#endif
    }
}