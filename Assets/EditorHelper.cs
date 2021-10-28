#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class EditorHelper
{
    #region Methods

    [MenuItem("Tools/Spawn 10K")]
    private static void Spawn10K()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/A.prefab");
        for (var i = 0; i < 10000; i++)
        {
            Object.Instantiate(prefab).name = $"A {i}";
        }
    }

    #endregion
}
#endif