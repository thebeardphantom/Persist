using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Persist
{
    [DisallowMultipleComponent]
    public partial class ScenePersistenceRoot : MonoBehaviour, IEnumerable<TrackedObject>
    {
        #region Properties

        [field: SerializeField]
        private List<TrackedObject> TrackedObjects { get; set; } = new();

        #endregion

        #region Methods

        public static bool TryFindForScene(
            Scene scene,
            out ScenePersistenceRoot scenePersistenceRoot,
            bool createIfNotExists = false)
        {
            if (!scene.IsValid())
            {
                throw new Exception($"Scene is invalid {scene}");
            }

            foreach (var spr in FindObjectsOfType<ScenePersistenceRoot>())
            {
                if (spr.gameObject.scene == scene)
                {
                    scenePersistenceRoot = spr;
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

        private static ScenePersistenceRoot CreateScenePersistenceRoot(Scene scene)
        {
            var obj = new GameObject("Scene Persistence Root");
            var scenePersistenceRoot = obj.AddComponent<ScenePersistenceRoot>();
            SceneManager.MoveGameObjectToScene(obj, scene);
#if UNITY_EDITOR
            scenePersistenceRoot.RefreshScene();
#endif
            return scenePersistenceRoot;
        }

        public SceneSaveBlob CreateSceneSaveBlob()
        {
            var sceneSaveBlob = new SceneSaveBlob(gameObject.scene.name, new List<ObjectSaveBlob>());
            foreach (var trackedObject in TrackedObjects)
            {
                var persistableScript = (IPersistableScript)trackedObject.Component;
                var persistData = persistableScript.Save();
                if (persistData == null)
                {
                    continue;
                }

                sceneSaveBlob.SaveBlobs.Add(new ObjectSaveBlob(trackedObject.ID, persistData));
            }

            return sceneSaveBlob;
        }

        public void LoadSceneSaveBlob(in SceneSaveBlob sceneSaveBlob)
        {
            foreach (var objectSaveBlob in sceneSaveBlob.SaveBlobs)
            {
                int trackedObjectIndex;
                for (trackedObjectIndex = 0; trackedObjectIndex < TrackedObjects.Count; trackedObjectIndex++)
                {
                    if (TrackedObjects[trackedObjectIndex].ID == objectSaveBlob.ID)
                    {
                        break;
                    }
                }

                if (trackedObjectIndex < 0 || trackedObjectIndex >= TrackedObjects.Count)
                {
                    continue;
                }

                var trackedObject = TrackedObjects[trackedObjectIndex];
                var persistableScript = (IPersistableScript)trackedObject.Component;
                persistableScript.Load(objectSaveBlob.PersistData);
            }
        }

        /// <inheritdoc />
        public IEnumerator<TrackedObject> GetEnumerator()
        {
            return TrackedObjects.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)TrackedObjects).GetEnumerator();
        }

        #endregion
    }
}