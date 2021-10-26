#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BeardPhantom.Identify
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class AutoIdentify : MonoBehaviour
    {
        #region Methods

        private void Awake()
        {
            hideFlags = HideFlags.DontSaveInBuild;
            if (!CanHaveScript())
            {
                DestroyImmediate(this, true);
            }
        }

        private bool CanHaveScript()
        {
            if (PrefabUtility.IsPartOfPrefabAsset(this) || EditorUtility.IsPersistent(this))
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }

            if (PrefabUtility.GetPrefabInstanceStatus(this) != PrefabInstanceStatus.NotAPrefab)
            {
                return true;
            }

            // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
            var gObj = gameObject;
            var mainStage = StageUtility.GetMainStageHandle();
            var currentStage = StageUtility.GetStageHandle(gObj);
            return currentStage != mainStage && PrefabStageUtility.GetPrefabStage(gObj) != null;
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            var gObj = gameObject;
            if (PrefabUtility.GetPrefabInstanceStatus(this) != PrefabInstanceStatus.NotAPrefab)
            {
                EditorApplication.delayCall += () =>
                {
                    ScenePersistenceRoot.TryAdd(gObj);
                };
            }
        }

        #endregion
    }
}
#endif