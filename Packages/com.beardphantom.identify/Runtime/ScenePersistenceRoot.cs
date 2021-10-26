using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Identify
{
    [DisallowMultipleComponent]
    public partial class ScenePersistenceRoot : MonoBehaviour, IEnumerable<TrackedObject>
    {
        #region Fields

        private static readonly List<GameObject> _rootList = new List<GameObject>();

        #endregion

        #region Properties

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        [field: SerializeField]
        private List<TrackedObject> TrackedObjects { get; set; } = new List<TrackedObject>();

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

                if (createIfNotExists && !Application.isPlaying)
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
            var obj = new GameObject("Scene Persistence Root");
            var scenePersistenceRoot = obj.AddComponent<ScenePersistenceRoot>();
            scenePersistenceRoot.ValidateHideFlags();
            SceneManager.MoveGameObjectToScene(obj, scene);
            EditorSceneManager.MarkSceneDirty(scene);
            return scenePersistenceRoot;
        }

        /// <inheritdoc />
        public IEnumerator<TrackedObject> GetEnumerator()
        {
            return TrackedObjects.GetEnumerator();
        }

        private bool IsTrackedInternal(GameObject gObj)
        {
            foreach (var trackedObj in TrackedObjects)
            {
                if (trackedObj.GameObject == gObj)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)TrackedObjects).GetEnumerator();
        }

        #endregion
    }
}