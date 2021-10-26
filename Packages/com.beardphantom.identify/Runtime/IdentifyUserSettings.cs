using UnityEngine;

public partial class IdentifyUserSettings : ScriptableObject
{
    #region Properties

    [field: SerializeField]
    public bool RevealPersistenceRoots { get; private set; }

    #endregion
}