using System.IO;
using UnityEditor;
using UnityEngine;

public class ResourceHelper
{
    public static GameObject LoadPrefab(string relaPath)
    {
        var rootPath = "Assets/Prefabs";
        var fullPath = Path.Join(rootPath, relaPath);
        
        return LoadAsset<GameObject>(fullPath);
    }
    
    private static T LoadAsset<T>(string assetFullPath) where T : Object
    {
#if UNITY_EDITOR
        return AssetDatabase.LoadAssetAtPath<T>(assetFullPath);
#else 
        // TODO
        return null;
#endif
    }
}
