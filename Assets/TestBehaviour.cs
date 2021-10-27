using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BeardPhantom.Identify
{
    public class TestBehaviour : MonoBehaviour, IPersistableScript
    {
        #region Types

        [Serializable]
        private class PersistData
        {
            #region Fields

            public int Test;

            #endregion
        }

        [Serializable]
        private class SaveTest
        {
            #region Fields

            public List<SceneSaveBlob> SceneSaveBlobs;

            #endregion
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public object Save()
        {
            return new PersistData
            {
                Test = Random.Range(0, 1000)
            };
        }

        /// <inheritdoc />
        public void Load(object data)
        {
            var persistData = (PersistData)data;
            Debug.Log(persistData.Test);
        }

        // private void Start()
        // {
        //     var sceneSaveBlobs = new List<SceneSaveBlob>();
        //     PersistUtility.CreateSceneSaveBlobs(sceneSaveBlobs);
        //     Debug.Log(JsonUtility.ToJson(new SaveTest
        //         {
        //             SceneSaveBlobs = sceneSaveBlobs
        //         },
        //         true));
        // }

        #endregion
    }
}