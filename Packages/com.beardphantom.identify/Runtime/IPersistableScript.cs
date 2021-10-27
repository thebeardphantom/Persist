namespace BeardPhantom.Identify
{
    public interface IPersistableScript
    {
        #region Methods

        object Save();

        void Load(object data);

        #endregion
    }
}