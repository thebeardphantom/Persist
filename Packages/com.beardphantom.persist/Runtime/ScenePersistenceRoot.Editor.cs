#if UNITY_EDITOR
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BeardPhantom.Persist
{
    [ExecuteAlways]
    [AddComponentMenu("")]
    public partial class ScenePersistenceRoot
    {
        #region Fields

        private static readonly List<GameObject> _sceneRoots = new();

        private static readonly List<IPersistableScript> _persistableScripts = new();

        private static readonly List<Object> _allPersistableScripts = new();

        #endregion

        #region Methods

        public void Rebuild()
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var scene = gameObject.scene;
                scene.GetRootGameObjects(_sceneRoots);

                foreach (var root in _sceneRoots)
                {
                    root.GetComponentsInChildren(true, _persistableScripts);
                    foreach (var persistableScript in _persistableScripts)
                    {
                        _allPersistableScripts.Add((Object)persistableScript);
                    }
                }

                TrackedObjects.Clear();
                if (_allPersistableScripts.Count > 0)
                {
                    var globalObjectIds = new GlobalObjectId[_allPersistableScripts.Count];
                    GlobalObjectId.GetGlobalObjectIdsSlow(_allPersistableScripts.ToArray(), globalObjectIds);

                    for (var i = 0; i < _allPersistableScripts.Count; i++)
                    {
                        var component = (Component)_allPersistableScripts[i];
                        var id = globalObjectIds[i].ToString();
                        TrackedObjects.Add(TrackedObject.Create(component, id));
                    }
                }

                Debug.Log($"Rebuild took {sw.Elapsed.TotalMilliseconds}ms");
                sw.Stop();
            }
            finally
            {
                _allPersistableScripts.Clear();
                _persistableScripts.Clear();
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            ValidateHideFlags();
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
            var settings = PersistUserSettings.GetOrCreateSettings();
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