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
        public Hash128 ID { get; private set; }

        #endregion

        #region Methods

        internal static TrackedObject Create(Component component, Hash128 id)
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