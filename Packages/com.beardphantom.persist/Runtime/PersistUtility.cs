using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Persist
{
    public static class PersistUtility
    {
        #region Methods

        public static void CreateSceneSaveBlobs(List<SceneSaveBlob> sceneSaveBlobs)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (ScenePersistenceRoot.TryFindForScene(scene, out var scenePersistenceRoot))
                {
                    sceneSaveBlobs.Add(scenePersistenceRoot.CreateSceneSaveBlob());
                }
            }
        }

        public static void LoadFromSceneSaveBlobs(List<SceneSaveBlob> sceneSaveBlobs)
        {
            foreach (var sceneSaveBlob in sceneSaveBlobs)
            {
                var scene = SceneManager.GetSceneByName(sceneSaveBlob.SceneName);
                if (ScenePersistenceRoot.TryFindForScene(scene, out var scenePersistenceRoot))
                {
                    scenePersistenceRoot.LoadSceneSaveBlob(sceneSaveBlob);
                }
            }
        }

        #endregion
    }
}