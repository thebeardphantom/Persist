using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;

public partial class IdentifyUserSettings
{
    #region Methods

    public static IdentifyUserSettings GetOrCreateSettings()
    {
        var settings = LoadFromDisk();
        if (settings == null)
        {
            settings = CreateInstance<IdentifyUserSettings>();
            SaveToDisk(settings);
        }

        return settings;
    }

    private static IdentifyUserSettings LoadFromDisk()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "UserSettings", "IdentifyUserSettings.asset");
        var settings = (IdentifyUserSettings)InternalEditorUtility.LoadSerializedFileAndForget(path).FirstOrDefault();
        return settings;
    }

    private static void SaveToDisk(IdentifyUserSettings settings)
    {
        var path = Path.Combine(Environment.CurrentDirectory, "UserSettings", "IdentifyUserSettings.asset");
        InternalEditorUtility.SaveToSerializedFileAndForget(new Object[]
            {
                settings
            },
            path,
            true);
    }

    [SettingsProvider]
    private static SettingsProvider CreateSettingsProvider()
    {
        return new SettingsProvider("Preferences/Identify", SettingsScope.User)
        {
            guiHandler = searchContext =>
            {
                var settings = GetOrCreateSettings();
                var serializedObject = new SerializedObject(settings);
                using var changeCheckScope = new EditorGUI.ChangeCheckScope();
                var revealProperty = serializedObject.FindProperty("<RevealPersistenceRoots>k__BackingField");
                EditorGUILayout.PropertyField(revealProperty);
                if (changeCheckScope.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    SaveToDisk(settings);
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            },
            keywords = new HashSet<string>(new[]
            {
                "Reveal Persistence Roots"
            })
        };
    }

    #endregion
}