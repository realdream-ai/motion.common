// author : https://github.com/JiepengTan/RealDreamTools

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RealDream
{
    // PrefabUtility
    public static partial class EditorExtUtil
    {
        public static UnityEngine.Object InstantiatePrefab(UnityEngine.Object assetComponentOrGameObject)
        {
#if UNITY_EDITOR
            return PrefabUtility.InstantiatePrefab(assetComponentOrGameObject);
#endif
            return GameObject.Instantiate(assetComponentOrGameObject);
        }

        public static GameObject SaveAsPrefabAsset(GameObject instanceRoot, string assetPath)
        {
#if UNITY_EDITOR
            return PrefabUtility.SaveAsPrefabAsset(instanceRoot, assetPath);
#endif
            return null;
        }
        public static void Destroy(GameObject go)
        {
            if(go == null) return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject.DestroyImmediate(go);
                return;
            }
#endif
            GameObject.Destroy(go);
        }
        
    }

    /// <summary>
    /// EditorUtility
    /// </summary>
    public static partial class EditorExtUtil
    {
        public static bool DisplayDialog(string title, string message, string ok,
            [UnityEngine.Internal.DefaultValue("\"\"")]
            string cancel)
        {
#if UNITY_EDITOR
            return UnityEditor.EditorUtility.DisplayDialog(title, message, ok, cancel);
#else
            return false;
#endif
        }
        public static void DisplayProgressBar(int idx, int totalCount)
        {
#if UNITY_EDITOR
            var title = $"{idx}/ {totalCount}";
            UnityEditor.EditorUtility.DisplayProgressBar(title, title, idx*1.0f/totalCount);
#endif
        }
        public static void DisplayProgressBar(string title, string info, float progress)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayProgressBar(title, info, progress);
#endif
        }

        public static void UnloadUnusedAssetsImmediate()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.UnloadUnusedAssetsImmediate();
#endif
        }

        public static void ClearProgressBar()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
#endif
        }
    }

    // AssetDatabase
    public static partial class EditorExtUtil
    {
        public static string CreateFolder(string parentFolder, string newFolderName)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.CreateFolder(parentFolder, newFolderName);
#else
        return "";
#endif
        }
        public static void DeleteAssets(string[] paths, List<string> outFailedPaths)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.DeleteAssets(paths,outFailedPaths);
#endif
        }
        
        public static bool IsValidFolder(string path)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.IsValidFolder(path);
#else
            return default;
#endif
        }

        public static string GetAssetPath(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(obj);
#else
            return "";
#endif
        }

        public static string[] GetAssetBundleDependencies(string assetBundleName, bool recursive)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetBundleDependencies(assetBundleName, recursive);
#else
        return null;
#endif
        }

        public static T LoadAssetAtPath<T>(string assetPath) where T : Object
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
#else
        return null;
#endif
        }

        public static string[] GetAssetPathsFromAssetBundle(string assetBundleName)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
#else
        return null;
#endif
        }

        public static void ToggleScriptingDefineSymbols(string def, bool isOn)
        {
#if UNITY_EDITOR
            ToggleScriptingDefineSymbols(def, isOn, (int)BuildTargetGroup.Standalone);
            ToggleScriptingDefineSymbols(def, isOn, (int)BuildTargetGroup.Android);
#endif
        }

        static void ToggleScriptingDefineSymbols(string def, bool isOn, int type)
        {
#if UNITY_EDITOR
            var targetGroup = (BuildTargetGroup)type;
            string ori = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            List<string> defineSymbols = new List<string>(ori.Split(';'));
            if (isOn)
            {
                if (!defineSymbols.Contains(def))
                {
                    defineSymbols.Add(def);
                }
            }
            else
            {
                defineSymbols.Remove(def);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", defineSymbols.ToArray()));
#endif
        }


        public static void SetDirty(ScriptableObject obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }

        public static void ImportAsset(string path)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.ImportAsset(path);
#endif
        }

        public static void Refresh()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }


        public static void DeleteAsset(string path)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.DeleteAsset(path);
#endif
        }

        public static void CreateAsset(UnityEngine.Object asset, string path)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(asset, path);
#endif
        }

        public static void SaveTblConfig(ScriptableObject confg)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(confg);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var path = AssetDatabase.GetAssetPath(confg);
            AssetDatabase.ImportAsset(path);
            AssetDatabase.ImportAsset(path.Replace(".asset", ".csv"));
            EditorWindow.focusedWindow?.ShowNotification(new GUIContent("Done"));
#endif
        }
        public static GameObject Instantiate(GameObject original)
        {
            return  Instantiate( original, Vector3.zero, Quaternion.identity, null);
        }
        public static GameObject Instantiate(
            GameObject original,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return Object.Instantiate(original, position, rotation, parent);
            }

            var go = PrefabUtility.InstantiatePrefab(original, parent) as GameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            return go;
#else
        return  Object.Instantiate( original, position, rotation, parent);
#endif
        }

        public static void StopGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public static bool isPlaying
        {
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorApplication.isPlaying;
#endif
                return false;
            }
        }
    }
}