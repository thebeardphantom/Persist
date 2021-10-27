using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Identify.Editor
{
    public class SceneProcessor : IProcessSceneWithReport
    {
        #region Properties

        /// <inheritdoc />
        public int callbackOrder { get; }

        #endregion

        #region Methods

        /// <inheritdoc />
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            IdentifyEditorUtility.HandleSceneRootRequirements(scene);
        }

        #endregion
    }
}