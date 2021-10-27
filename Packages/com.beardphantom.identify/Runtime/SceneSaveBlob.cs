using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.Identify
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

    [Serializable]
    public struct ObjectSaveBlob
    {
        #region Fields

        public Hash128 ObjectId;

        public object PersistData;

        #endregion

        #region Constructors

        public ObjectSaveBlob(Hash128 objectId, object persistData)
        {
            ObjectId = objectId;
            PersistData = persistData;
        }

        #endregion
    }
}