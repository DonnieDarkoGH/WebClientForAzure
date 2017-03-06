using UnityEngine;
using System.Collections;
using UnityEditor;

public static class ScriptableObjectMaker {

    [MenuItem("Assets/Create/ProfilesData")]
    public static void CreateMyAsset() {

        LocalApplicationData asset = ScriptableObject.CreateInstance<LocalApplicationData>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/ProfilesData.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
