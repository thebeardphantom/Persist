using System;
using System.Collections.Generic;

namespace BeardPhantom.Persist
{
    [Serializable]
    public struct SceneSaveBlob
    {
        #region Fields

        public string SceneName;

        public List<ObjectSaveBlob> SaveBlobs;

        #endregion

        #region Constructors

        public SceneSaveBlob(string sceneName, List<ObjectSaveBlob> saveBlobs)
        {
            SceneName = sceneName;
            SaveBlobs = saveBlobs;
        }

        #endregion
    }
}