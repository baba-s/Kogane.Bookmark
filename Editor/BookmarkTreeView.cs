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
        private enum ColumnType
        {
            PING,
            NAME,
        }

        // 要素が存在しない場合、 TreeView は例外を発生する
        // そのため、要素が存在しない場合は表示しないダミーデータを追加する
        private static BookmarkTreeViewItem[] List =>
            BookmarkSetting.instance
                .Where( x => x != null )
                .Select( ( x, index ) => new BookmarkTreeViewItem( index, x ) )
                .DefaultIfEmpty( new BookmarkTreeViewItem( 0, null ) )
                .ToArray();

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
            var root = new TreeViewItem { depth = -1 };

            foreach ( var n in List )
            {
                root.AddChild( n );
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
                        var name = "eyeDropper.Large";
                        var icon = EditorGUIUtility.IconContent( name );

                        if ( GUI.Button( rect, icon.image, EditorStyles.iconButton ) )
                        {
                            EditorGUIUtility.PingObject( asset );
                        }

                        break;

                    case ColumnType.NAME:
                        EditorGUIUtility.SetIconSize( new Vector2( 16, 16 ) );
                        var label = EditorGUIUtility.ObjectContent( asset, null );
                        EditorGUI.LabelField( rect, label );
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

            return item.Name.IndexOf( search, StringComparison.OrdinalIgnoreCase ) != -1;
        }

        /// <summary>
        /// ソートされる時に呼び出されます
        /// </summary>
        private void SortItems( MultiColumnHeader header )
        {
            var ascending = header.IsSortedAscending( header.sortedColumnIndex );
            var list      = ascending ? List : List.Reverse();

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
            var bookmarkData = List.FirstOrDefault( x => x.id == id );

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