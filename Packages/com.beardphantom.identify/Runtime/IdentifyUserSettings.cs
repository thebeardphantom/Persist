using UnityEngine;

namespace BeardPhantom.Identify
{
    public partial class IdentifyUserSettings : ScriptableObject
    {
        #region Properties

        [field: SerializeField]
        public bool RevealPersistenceRoots { get; private set; }

        #endregion
    }
}