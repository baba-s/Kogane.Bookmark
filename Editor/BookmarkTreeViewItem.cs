using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kogane.Internal
{
    /// <summary>
    /// ブックマークの項目を管理するクラス
    /// </summary>
    internal sealed class BookmarkTreeViewItem : TreeViewItem
    {
        //==============================================================================
        // プロパティ
        //==============================================================================
        public Object Asset { get; }

        public bool   IsValid   => Asset != null;
        public string Name      => IsValid ? Asset.name : string.Empty;
        public string AssetPath => AssetDatabase.GetAssetPath( Asset );
        public bool   IsFolder  => AssetDatabase.IsValidFolder( AssetPath );

        //==============================================================================
        // 関数
        //==============================================================================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BookmarkTreeViewItem( int id, Object asset ) : base( id )
        {
            Asset = asset;
        }
    }
}