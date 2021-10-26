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
        if (IsTrackedInternal(gObj))
        {
            return;
        }

        Undo.RecordObject(this, $"Enable tracking {gObj}");
        TrackedObjects.Add(TrackedObject.Create(gObj, CreateID(gObj)));
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        EditorApplication.RepaintHierarchyWindow();
    }

    private void TryRemoveInternal(GameObject gObj)
    {
        var index = TrackedObjects.FindIndex(tracked => tracked.GameObject == gObj);
        if (index >= 0)
        {
            RemoveAtIndex(index);
        }
    }

    private void RemoveAtIndex(int index)
    {
        var trackedObject = TrackedObjects[index];
        Undo.RecordObject(this, $"Disable tracking {trackedObject.GameObject}");
        TrackedObjects.RemoveAt(index);

        if (TrackedObjects.Count == 0)
        {
            Undo.DestroyObjectImmediate(gameObject);
        }
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        // Check if GameObject has been moved to another scene or has been destroyed
        for (var i = TrackedObjects.Count - 1; i >= 0; i--)
        {
            var trackedObject = TrackedObjects[i];
            if (trackedObject.GameObject == null || trackedObject.GameObject.scene != gameObject.scene)
            {
                RemoveAtIndex(i);
            }
        }
    }

    private void OnValidate()
    {
        EditorApplication.delayCall += () =>
        {
            // Delay for multiple reasons. On first creation there will be no tracked objects,
            // meaning this object will be destroyed immediately.
            // Secondly, you cannot call DestroyImmediate in OnValidate().
            if (TrackedObjects.Count == 0)
            {
                Undo.DestroyObjectImmediate(gameObject);
            }
        };
    }

    private void OnDestroy()
    {
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
        EditorApplication.RepaintHierarchyWindow();
    }

    #endregion
}

#endif