using System;
using UnityEngine;

namespace BeardPhantom.Identify
{
    [Serializable]
    public struct TrackedObject
    {
        #region Properties

        [field: SerializeField]
        public GameObject GameObject { get; private set; }

        [field: SerializeField]
        public Hash128 ID { get; private set; }

        #endregion

        #region Methods

        internal static TrackedObject Create(GameObject gameObject, Hash128 id)
        {
            return new TrackedObject
            {
                GameObject = gameObject,
                ID = id
            };
        }

        #endregion
    }
}