using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kogane.Internal
{
    /// <summary>
    /// アセットをブックマークできるウィンドウ
    /// </summary>
    internal sealed class BookmarkWindow : EditorWindow
    {
        //==============================================================================
        // 定数(const)
        //==============================================================================
        private const string SEARCH_STRING_STATE_KEY = "BookmarkWindow_SearchString";

        //==============================================================================
        // 定数(static readonly)
        //==============================================================================
        private static readonly GUILayoutOption   WIDTH_OPTION  = GUILayout.Width( 16 );
        private static readonly GUILayoutOption   HEIGHT_OPTION = GUILayout.Height( EditorGUIUtility.singleLineHeight );
        private static readonly GUILayoutOption[] OPTIONS       = { WIDTH_OPTION, HEIGHT_OPTION };

        //================================================================================
        // 変数
        //================================================================================
        private SearchField      m_searchField;
        private BookmarkHeader   m_header;
        private BookmarkTreeView m_treeView;

        //================================================================================
        // 変数(static)
        //================================================================================
        private static GUIContent m_refreshIcon;
        private static GUIContent m_settingIcon;

        //================================================================================
        // 関数
        //================================================================================
        /// <summary>
        /// 有効になった時に呼び出されます
        /// </summary>
        private void OnEnable()
        {
            var state = new TreeViewState();

            m_header = new BookmarkHeader( null );

            m_treeView = new BookmarkTreeView( state, m_header )
            {
                searchString = SessionState.GetString( SEARCH_STRING_STATE_KEY, string.Empty )
            };

            m_searchField                         =  new SearchField();
            m_searchField.downOrUpArrowKeyPressed += m_treeView.SetFocusAndEnsureSelectedItem;

            BookmarkSetting.OnChanged += OnChanged;
            Undo.undoRedoPerformed    += OnChanged;
        }

        private void OnDisable()
        {
            BookmarkSetting.OnChanged -= OnChanged;
            Undo.undoRedoPerformed    -= OnChanged;
        }

        private void OnChanged()
        {
            m_treeView.Reload();
        }

        /// <summary>
        /// GUI を描画する時に呼び出されます
        /// </summary>
        private void OnGUI()
        {
            using ( new EditorGUILayout.HorizontalScope() )
            {
                DrawAddAssetButton();
                DrawSearchField();
                DrawRefreshButton();
                DrawSettingButton();
            }

            DrawTreeView();
            DragTo();
        }

        /// <summary>
        /// アセット追加ボタンを描画します
        /// </summary>
        private void DrawAddAssetButton()
        {
            var rect = new Rect
            {
                x      = 0,
                y      = 0,
                width  = position.width / 3,
                height = EditorGUIUtility.singleLineHeight - 1
            };

            GUILayout.Box( "Drag Here", EditorStyles.toolbarButton, GUILayout.Width( rect.width ) );

            var current = Event.current;

            switch ( current.type )
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if ( !rect.Contains( current.mousePosition ) ) break;

                    DragAndDrop.visualMode      = DragAndDropVisualMode.Copy;
                    DragAndDrop.activeControlID = GUIUtility.GetControlID( FocusType.Passive );

                    if ( current.type == EventType.DragPerform )
                    {
                        DragAndDrop.AcceptDrag();

                        foreach ( var draggedObject in DragAndDrop.objectReferences )
                        {
                            var assetPath = AssetDatabase.GetAssetPath( draggedObject );
                            var asset     = AssetDatabase.LoadAssetAtPath<Object>( assetPath );

                            BookmarkSetting.instance.Add( asset );
                        }

                        ReloadTreeView();

                        DragAndDrop.activeControlID = 0;
                    }

                    current.Use();
                    break;
            }
        }

        private void DrawRefreshButton()
        {
            m_refreshIcon ??= EditorGUIUtility.IconContent( "d_Refresh" );

            using var scope = new EditorGUILayout.VerticalScope( OPTIONS );

            GUILayout.FlexibleSpace();

            if ( GUILayout.Button( m_refreshIcon, EditorStyles.iconButton ) )
            {
                ReloadTreeView();
            }

            GUILayout.FlexibleSpace();
        }

        private static void DrawSettingButton()
        {
            m_settingIcon ??= EditorGUIUtility.IconContent( "d_SettingsIcon" );

            using var scope = new EditorGUILayout.VerticalScope( OPTIONS );

            GUILayout.FlexibleSpace();

            if ( GUILayout.Button( m_settingIcon, EditorStyles.iconButton ) )
            {
                SettingsService.OpenProjectSettings( BookmarkSettingProvider.PATH );
            }

            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// TreeView をリロードします
        /// </summary>
        private void ReloadTreeView()
        {
            m_treeView.Reload();

            var index = m_header.sortedColumnIndex;
            m_header.SetSorting( index, !m_header.IsSortedAscending( index ) );
            m_header.SetSorting( index, !m_header.IsSortedAscending( index ) );
        }

        /// <summary>
        /// 検索欄を描画します
        /// </summary>
        private void DrawSearchField()
        {
            using var scope = new EditorGUI.ChangeCheckScope();

            if ( m_treeView == null ) return;

            var searchString = m_searchField.OnToolbarGUI( m_treeView.searchString );

            if ( !scope.changed ) return;

            SessionState.SetString( SEARCH_STRING_STATE_KEY, searchString );
            m_treeView.searchString = searchString;
        }

        /// <summary>
        /// ツリービューを描画します
        /// </summary>
        private void DrawTreeView()
        {
            var singleLineHeight = EditorGUIUtility.singleLineHeight;

            var rect = new Rect
            {
                x      = 0,
                y      = singleLineHeight + 1,
                width  = position.width,
                height = position.height - singleLineHeight - 1
            };

            m_treeView.OnGUI( rect );
        }

        /// <summary>
        /// アセットを Hierarchy や Project ビューにドラッグする時に呼び出します
        /// </summary>
        private void DragTo()
        {
            if ( Event.current.type != EventType.MouseDrag ) return;

            var array = BookmarkSetting.instance.ToArray();

            var selectedPrefabs = m_treeView
                    .GetSelection()
                    .Select( x => array[ x ] )
                    .Where( x => x != null )
                    .ToArray()
                ;

            if ( selectedPrefabs.Length <= 0 ) return;

            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = selectedPrefabs;
            DragAndDrop.StartDrag( "Dragging" );

            Event.current.Use();
        }

        //================================================================================
        // 関数(static)
        //================================================================================
        /// <summary>
        /// 開きます
        /// </summary>
        [MenuItem( "Window/Kogane/Bookmark", false, 1155441539 )]
        private static void Open()
        {
            GetWindow<BookmarkWindow>( "Bookmark" );
        }
    }
}