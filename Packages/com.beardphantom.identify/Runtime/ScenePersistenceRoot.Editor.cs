#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteAlways]
[AddComponentMenu("")]
public partial class ScenePersistenceRoot
{
    #region Methods

    public static void TryAdd(GameObject gObj)
    {
        if (gObj.scene.IsValid())
        {
            TryFindForScene(gObj.scene, out var scenePersistenceRoot, true);
            scenePersistenceRoot.TryAddInternal(gObj);
        }
    }

    public static void TryRemove(GameObject gObj)
    {
        if (gObj.scene.IsValid() && TryFindForScene(gObj.scene, out var scenePersistenceRoot))
        {
            scenePersistenceRoot.TryRemoveInternal(gObj);
        }
    }

    private static PropertyName CreateID(Object gObj)
    {
        var globalObjectId = GlobalObjectId.GetGlobalObjectIdSlow(gObj);
        return new PropertyName(globalObjectId.ToString());
    }

    private void TryAddInternal(GameObject gObj)
    {
        if (TrackedObjects.Contains(gObj))
        {
            return;
        }

        Undo.RecordObject(this, $"Enable tracking {gObj}");
        TrackedObjects.Add(gObj);
        TrackedIDs.Add(CreateID(gObj));
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        EditorApplication.RepaintHierarchyWindow();
    }

    private void TryRemoveInternal(GameObject gObj)
    {
        var index = TrackedObjects.IndexOf(gObj);
        if (index >= 0)
        {
            Undo.RecordObject(this, $"Disable tracking {gObj}");
            TrackedObjects.RemoveAt(index);
            TrackedIDs.RemoveAt(index);

            if (TrackedObjects.Count == 0)
            {
                Undo.DestroyObjectImmediate(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        EditorApplication.RepaintHierarchyWindow();
    }

    #endregion
}

#endif