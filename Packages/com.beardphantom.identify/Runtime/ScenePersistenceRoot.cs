using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public partial class ScenePersistenceRoot : MonoBehaviour
{
    #region Fields

    private static readonly List<GameObject> _rootList = new List<GameObject>();

    #endregion

    #region Properties

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    [field: SerializeField]
    private List<GameObject> TrackedObjects { get; set; } = new List<GameObject>();

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    [field: SerializeField]
    private List<PropertyName> TrackedIDs { get; set; } = new List<PropertyName>();

    #endregion

    #region Methods

    public static bool IsTracked(GameObject gObj)
    {
        return gObj.scene.IsValid()
            && TryFindForScene(gObj.scene, out var scenePersistenceRoot)
            && scenePersistenceRoot.IsTrackedInternal(gObj);
    }

    public static bool TryFindForScene(
        Scene scene,
        out ScenePersistenceRoot scenePersistenceRoot,
        bool createIfNotExists = false)
    {
        if (!scene.IsValid())
        {
            throw new Exception($"Scene is invalid {scene}");
        }

        try
        {
            scene.GetRootGameObjects(_rootList);
            foreach (var root in _rootList)
            {
                if (root.TryGetComponent(out scenePersistenceRoot))
                {
                    return true;
                }
            }

            if (createIfNotExists)
            {
                scenePersistenceRoot = CreateScenePersistenceRoot(scene);
                return true;
            }

            scenePersistenceRoot = default;
            return false;
        }
        finally
        {
            _rootList.Clear();
        }
    }

    private static ScenePersistenceRoot CreateScenePersistenceRoot(Scene scene)
    {
        var obj = new GameObject("Scene Persistence Root")
        {
            hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable
        };
        var scenePersistenceRoot = obj.AddComponent<ScenePersistenceRoot>();
        scenePersistenceRoot.hideFlags = HideFlags.NotEditable;
        SceneManager.MoveGameObjectToScene(obj, scene);
        EditorSceneManager.MarkSceneDirty(scene);
        return scenePersistenceRoot;
    }

    private bool IsTrackedInternal(GameObject gObj)
    {
        return TrackedObjects.Contains(gObj);
    }

    #endregion
}