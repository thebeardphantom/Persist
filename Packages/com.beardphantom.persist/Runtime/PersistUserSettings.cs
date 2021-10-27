using UnityEngine;

namespace BeardPhantom.Persist
{
    public partial class PersistUserSettings : ScriptableObject
    {
        #region Properties

        [field: SerializeField]
        public bool RevealPersistenceRoots { get; private set; }

        #endregion
    }
}