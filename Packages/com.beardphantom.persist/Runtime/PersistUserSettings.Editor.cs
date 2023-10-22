#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;

namespace BeardPhantom.Persist
{
    public partial class PersistUserSettings
    {
        #region Fields

        private static readonly string _revealPersistenceRootsPropertyPath = $"<{nameof(RevealPersistenceRoots)}>k__BackingField";

        #endregion

        #region Methods

        public static PersistUserSettings GetOrCreateSettings()
        {
            var settings = LoadFromDisk();
            if (settings == null)
            {
                settings = CreateInstance<PersistUserSettings>();
                SaveToDisk(settings);
            }

            return settings;
        }

        private static string GetSettingsPath()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "UserSettings", "PersistUserSettings.asset");
            return path;
        }

        private static PersistUserSettings LoadFromDisk()
        {
            var path = GetSettingsPath();
            var settings = InternalEditorUtility.LoadSerializedFileAndForget(path)
                .FirstOrDefault() as PersistUserSettings;
            return settings;
        }

        private static void SaveToDisk(Object settings)
        {
            var path = GetSettingsPath();
            InternalEditorUtility.SaveToSerializedFileAndForget(
                new[]
                {
                    settings
                },
                path,
                true);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Preferences/Persist", SettingsScope.User)
            {
                guiHandler = searchContext =>
                {
                    var settings = GetOrCreateSettings();
                    var serializedObject = new SerializedObject(settings);
                    using var changeCheckScope = new EditorGUI.ChangeCheckScope();
                    var revealProperty = serializedObject.FindProperty(_revealPersistenceRootsPropertyPath);
                    EditorGUILayout.PropertyField(revealProperty);
                    if (changeCheckScope.changed)
                    {
                        serializedObject.ApplyModifiedProperties();
                        SaveToDisk(settings);
                        EditorApplication.QueuePlayerLoopUpdate();
                    }
                },
                keywords = new HashSet<string>(
                    new[]
                    {
                        "Reveal Persistence Roots"
                    })
            };
        }

        #endregion
    }
}
#endif