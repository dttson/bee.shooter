using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetUtils
{
    [MenuItem("EnemyData/Create new data")]
    public static void createEnemyData()
    {
        EnemyData asset = ScriptableObject.CreateInstance<EnemyData>();
        AssetDatabase.CreateAsset(asset, "Assets/_Master/_Datas/EnemyData.customasset");
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
