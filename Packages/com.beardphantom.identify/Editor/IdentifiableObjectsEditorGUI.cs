using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Identify.Editor
{
    [InitializeOnLoad]
    internal static class IdentifiableObjectsEditorGUI
    {
        #region Fields

        private const string TOOLTIP = "Click to toggle GameObject tracking. Context-click for additional options.";

        private static readonly Color _trackedColor = new Color(1f, 0f, 0f, 0.75f);

        private static readonly Color _untrackedColor = new Color(1f, 1f, 1f, 0.125f);

        private static bool _isGUIInitialized;

        private static GUIContent _trackedIconHeaderContent;

        private static GUIContent _trackedIconHierarchyContent;

        private static GUIStyle _style;

        #endregion

        #region Constructors

        static IdentifiableObjectsEditorGUI()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI += OnFinishedDefaultHeaderGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemOnGUI;
        }

        #endregion

        #region Methods

        private static void OnHierarchyItemOnGUI(int instanceID, Rect selectionRect)
        {
            var instanceIDToObject = EditorUtility.InstanceIDToObject(instanceID);
            var target = (GameObject)instanceIDToObject;
            if (target == null || !ScenePersistenceRoot.IsTracked(target))
            {
                return;
            }

            TryInitializeGUI();
            var size = selectionRect.size;
            size.x = size.y;
            var offset = new Vector2(-16, 0);

            SetupContent(ref selectionRect, size, offset);
            using (new GUIContentColorScope(_trackedColor))
            {
                GUI.Label(selectionRect, _trackedIconHeaderContent);
            }
        }

        private static void OnFinishedDefaultHeaderGUI(UnityEditor.Editor editor)
        {
            var target = editor.target as GameObject;
            if (target == null)
            {
                return;
            }

            var isTracked = ScenePersistenceRoot.IsTracked(target);
            using (new GUIContentColorScope(isTracked ? _trackedColor : _untrackedColor))
            {
                TryInitializeGUI();
                var rect = EditorGUILayout.GetControlRect(true, 2f);
                var size = new Vector2(20f, 20f);
                var offset = new Vector2(5f, -14f);
                SetupContent(ref rect, size, offset);
                if (GUI.Button(rect, _trackedIconHeaderContent, _style))
                {
                    if (Event.current.button == 1)
                    {
                        var hasScenePersistenceRoot = ScenePersistenceRoot.TryFindForScene(target.scene,
                            out var scenePersistenceRoot);
                        var menu = new GenericMenu();
                        var content = new GUIContent("Untrack All Scene Objects");
                        if (hasScenePersistenceRoot)
                        {
                            menu.AddGenericMenuItem(content,
                                scenePersistenceRoot.gameObject,
                                Undo.DestroyObjectImmediate);
                        }
                        else
                        {
                            menu.AddDisabledItem(content);
                        }

                        menu.ShowAsContext();
                    }
                    else if (isTracked)
                    {
                        foreach (var t in editor.targets.OfType<GameObject>())
                        {
                            ScenePersistenceRoot.TryRemove(t);
                        }
                    }
                    else
                    {
                        foreach (var t in editor.targets.OfType<GameObject>())
                        {
                            ScenePersistenceRoot.TryAdd(t);
                        }
                    }
                }
            }
        }

        private static void TryInitializeGUI()
        {
            if (_isGUIInitialized)
            {
                return;
            }

            var texture =
                AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.beardphantom.identify/Editor/Target.png");
            _trackedIconHierarchyContent = new GUIContent(texture);
            _trackedIconHeaderContent = new GUIContent(_trackedIconHierarchyContent)
            {
                tooltip = TOOLTIP
            };
            _style = GUI.skin.FindStyle("IconButton");
            _isGUIInitialized = true;
        }

        private static void SetupContent(ref Rect rect, Vector2 size, Vector2 offset)
        {
            const int PADDING = 2;
            _style.fixedWidth = size.x + PADDING;
            _style.fixedHeight = size.y + PADDING;
            _style.padding = new RectOffset(PADDING, PADDING, PADDING, PADDING);
            rect.width = size.x;
            rect.position += offset;
            rect.height = size.y;
        }

        private static void AddGenericMenuItem<T>(this GenericMenu menu, GUIContent content, T arg, Action<T> callback)
        {
            menu.AddItem(content,
                false,
                rawArg =>
                {
                    callback((T)rawArg);
                },
                arg);
        }

        #endregion
    }
}