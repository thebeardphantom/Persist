using System;
using UnityEngine;

[Serializable]
public struct TrackedObject
{
    #region Properties

    [field: SerializeField]
    public GameObject GameObject { get; private set; }

    [field: SerializeField]
    public PropertyName ID { get; private set; }

    #endregion

    #region Methods

    internal static TrackedObject Create(GameObject gameObject, PropertyName id)
    {
        return new TrackedObject
        {
            GameObject = gameObject,
            ID = id
        };
    }

    #endregion
}