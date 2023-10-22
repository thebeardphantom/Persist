using BeardPhantom.Persist;
using Newtonsoft.Json;
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
        public class PersistData
        {
            #region Fields

            public int Test;

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

        private void Start()
        {
            var sceneSaveBlobs = new List<SceneSaveBlob>();
            PersistUtility.CreateSceneSaveBlobs(sceneSaveBlobs);
            var json = JsonConvert.SerializeObject(
                new SaveTest
                {
                    SceneSaveBlobs = sceneSaveBlobs
                },
                Formatting.Indented);
            Debug.Log(json);
        }

        #endregion
    }
}