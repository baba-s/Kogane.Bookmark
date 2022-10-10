﻿using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kogane.Internal
{
    /// <summary>
    /// ブックマークウィンドウのツリービューを管理するクラス
    /// </summary>
    internal sealed class BookmarkTreeView : TreeView
    {
        //==============================================================================
        // 列挙型
        //==============================================================================
        /// <summary>
        /// 列の種類
        /// </summary>
        private enum ColumnType
        {
            PING,
            NAME,
            REMOVE,
        }

        //==============================================================================
        // 変数
        //==============================================================================
        private BookmarkTreeViewItem[] m_list;

        //==============================================================================
        // 変数(static)
        //==============================================================================
        private static GUIContent m_pingIcon;
        private static GUIContent m_removeIcon;

        //==============================================================================
        // 関数
        //==============================================================================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BookmarkTreeView
        (
            TreeViewState     state,
            MultiColumnHeader header
        ) : base( state, header )
        {
            rowHeight                     =  16;
            showAlternatingRowBackgrounds =  true;
            header.sortingChanged         += SortItems;

            header.ResizeToFit();
            Reload();
            header.SetSorting( 0, true );
        }

        /// <summary>
        /// ツリーを作成します
        /// </summary>
        protected override TreeViewItem BuildRoot()
        {
            // 要素が存在しない場合、 TreeView は例外を発生する
            // そのため、要素が存在しない場合は表示しないダミーデータを追加する
            m_list = BookmarkSetting.instance
                    .Where( x => x != null )
                    .Select( ( x, index ) => new BookmarkTreeViewItem( index, x ) )
                    .DefaultIfEmpty( new BookmarkTreeViewItem( 0, null ) )
                    .ToArray()
                ;

            var root = new TreeViewItem { depth = -1 };

            foreach ( var x in m_list )
            {
                root.AddChild( x );
            }

            return root;
        }

        /// <summary>
        /// 各行の GUI を描画します
        /// </summary>
        protected override void RowGUI( RowGUIArgs args )
        {
            if ( !( args.item is BookmarkTreeViewItem item ) || !item.IsValid ) return;

            var asset   = item.Asset;
            var columns = args.GetNumVisibleColumns();

            for ( var i = 0; i < columns; i++ )
            {
                var rect        = args.GetCellRect( i );
                var columnIndex = ( ColumnType )args.GetColumn( i );

                switch ( columnIndex )
                {
                    case ColumnType.PING:
                        m_pingIcon ??= EditorGUIUtility.IconContent( "eyeDropper.Large" );

                        if ( GUI.Button( rect, m_pingIcon.image, EditorStyles.iconButton ) )
                        {
                            EditorGUIUtility.PingObject( asset );
                        }

                        break;

                    case ColumnType.NAME:
                        EditorGUIUtility.SetIconSize( new Vector2( 16, 16 ) );
                        var label = EditorGUIUtility.ObjectContent( asset, null );
                        EditorGUI.LabelField( rect, label );
                        break;

                    case ColumnType.REMOVE:
                        m_removeIcon ??= EditorGUIUtility.IconContent( "eyeDropper.Large" );

                        if ( GUI.Button( rect, m_removeIcon.image, EditorStyles.iconButton ) )
                        {
                            BookmarkSetting.instance.Remove( asset );
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// 検索される時に呼び出されます
        /// </summary>
        protected override bool DoesItemMatchSearch( TreeViewItem treeViewItem, string search )
        {
            if ( treeViewItem is not BookmarkTreeViewItem item ) return false;
            if ( string.IsNullOrEmpty( search ) ) return true;

            return item.Name.IndexOf( search, StringComparison.OrdinalIgnoreCase ) != -1;
        }

        /// <summary>
        /// ソートされる時に呼び出されます
        /// </summary>
        private void SortItems( MultiColumnHeader header )
        {
            var ascending = header.IsSortedAscending( header.sortedColumnIndex );
            var list      = ascending ? m_list : m_list.Reverse();

            rootItem.children = list
                    .Cast<TreeViewItem>()
                    .ToList()
                ;

            BuildRows( rootItem );
        }

        /// <summary>
        /// ダブルクリックされた時に呼び出されます
        /// </summary>
        protected override void DoubleClickedItem( int id )
        {
            var bookmarkData = m_list.FirstOrDefault( x => x.id == id );

            if ( bookmarkData == null ) return;

            var asset    = bookmarkData.Asset;
            var isFolder = bookmarkData.IsFolder;

            if ( isFolder )
            {
                EditorGUIUtility.PingObject( asset );
            }
            else
            {
                AssetDatabase.OpenAsset( asset );
            }
        }
    }
}