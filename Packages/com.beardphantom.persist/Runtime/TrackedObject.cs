using System;
using UnityEngine;

namespace BeardPhantom.Persist
{
    [Serializable]
    public struct TrackedObject
    {
        #region Properties

        [field: SerializeField]
        public Component Component { get; private set; }

        [field: SerializeField]
        public string ID { get; private set; }

        #endregion

        #region Methods

        internal static TrackedObject Create(Component component, string id)
        {
            return new TrackedObject
            {
                Component = component,
                ID = id
            };
        }

        #endregion
    }
}