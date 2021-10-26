#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BeardPhantom.Identify
{
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

        private static Hash128 CreateID(Object gObj)
        {
            var globalObjectId = GlobalObjectId.GetGlobalObjectIdSlow(gObj);
            return Hash128.Compute(globalObjectId.ToString());
        }

        private void TryAddInternal(GameObject gObj)
        {
            if (IsTrackedInternal(gObj))
            {
                return;
            }

            Undo.RecordObject(this, $"Enable tracking {gObj}");
            TrackedObjects.Add(TrackedObject.Create(gObj, CreateID(gObj)));
            RefreshScene();
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

            ValidateHideFlags();

            // Check if GameObject has been moved to another scene or has been destroyed
            for (var i = TrackedObjects.Count - 1; i >= 0; i--)
            {
                var trackedObject = TrackedObjects[i];
                var trackedGObj = trackedObject.GameObject;
                if (trackedGObj == null || trackedGObj.scene != gameObject.scene)
                {
                    RemoveAtIndex(i);
                }

                if (!trackedObject.ID.isValid)
                {
                    TrackedObjects[i] = TrackedObject.Create(trackedGObj, CreateID(trackedGObj));
                }
            }
        }

        private void OnValidate()
        {
            ValidateHideFlags();
        }

        private void OnDestroy()
        {
            RefreshScene();
        }

        private void RefreshScene()
        {
            if (Application.isPlaying)
            {
                return;
            }

            EditorSceneManager.MarkSceneDirty(gameObject.scene);
            EditorApplication.RepaintHierarchyWindow();
        }

        private void OnTransformParentChanged()
        {
            transform.SetParent(null);
        }

        private void ValidateHideFlags()
        {
            var settings = IdentifyUserSettings.GetOrCreateSettings();
            var revealPersistenceRoots = settings.RevealPersistenceRoots;

            var newHideFlags = revealPersistenceRoots ? HideFlags.NotEditable : HideFlags.HideInHierarchy;
            if (gameObject.hideFlags != newHideFlags)
            {
                gameObject.hideFlags = newHideFlags;
                if (newHideFlags == HideFlags.NotEditable)
                {
                    hideFlags = HideFlags.NotEditable;
                }

                if (Selection.activeObject == gameObject)
                {
                    Selection.activeObject = null;
                }

                RefreshScene();
            }
        }

        #endregion
    }
}

#endif