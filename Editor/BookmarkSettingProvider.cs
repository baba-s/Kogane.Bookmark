using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kogane.Internal
{
    internal sealed class BookmarkSettingProvider : SettingsProvider
    {
        public const string PATH = "Kogane/Bookmark";

        private Editor m_editor;

        private BookmarkSettingProvider
        (
            string              path,
            SettingsScope       scopes,
            IEnumerable<string> keywords = null
        ) : base( path, scopes, keywords )
        {
        }

        public override void OnActivate( string searchContext, VisualElement rootElement )
        {
            var instance = BookmarkSetting.instance;

            instance.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;

            Editor.CreateCachedEditor( instance, null, ref m_editor );
        }

        public override void OnGUI( string searchContext )
        {
            using var changeCheckScope = new EditorGUI.ChangeCheckScope();

            var instance = BookmarkSetting.instance;
            var oldCount = instance.Count;

            m_editor.OnInspectorGUI();

            if ( !changeCheckScope.changed && oldCount == instance.Count ) return;

            BookmarkSetting.instance.Save();
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingProvider()
        {
            return new BookmarkSettingProvider
            (
                path: PATH,
                scopes: SettingsScope.Project
            );
        }
    }
}