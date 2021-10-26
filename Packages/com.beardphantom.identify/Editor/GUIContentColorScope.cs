using System;
using UnityEngine;

internal readonly struct GUIContentColorScope : IDisposable
{
    #region Fields

    private readonly Color _cachedContentColor;

    #endregion

    #region Constructors

    public GUIContentColorScope(Color color)
    {
        _cachedContentColor = GUI.contentColor;
        GUI.contentColor = color;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public void Dispose()
    {
        GUI.contentColor = _cachedContentColor;
    }

    #endregion
}