using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Persist.Editor
{
    [InitializeOnLoad]
    public static class PersistEditorUtility
    {
        #region Constructors

        static PersistEditorUtility()
        {
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        #endregion

        #region Methods

        public static void HandleSceneRootRequirements(Scene scene)
        {
            if (!scene.isLoaded)
            {
                return;
            }

            ScenePersistenceRoot.TryFindForScene(scene, out var scenePersistenceRoot, true);
            scenePersistenceRoot.Rebuild();
            if (!scenePersistenceRoot.Any())
            {
                Object.DestroyImmediate(scenePersistenceRoot.gameObject);
            }
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            HandleSceneRootRequirements(scene);
        }

        private static void OnSceneSaving(Scene scene, string path)
        {
            HandleSceneRootRequirements(scene);
        }

        #endregion
    }
}