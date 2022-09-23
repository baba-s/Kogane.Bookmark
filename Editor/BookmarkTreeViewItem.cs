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
        public Object Asset    { get; }
        public bool   IsValid  { get; }
        public string Name     { get; }
        public bool   IsFolder { get; }

        //==============================================================================
        // 関数
        //==============================================================================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BookmarkTreeViewItem( int id, Object asset ) : base( id )
        {
            var assetPath = AssetDatabase.GetAssetPath( asset );

            Asset    = asset;
            IsValid  = Asset != null;
            Name     = IsValid ? Asset.name : string.Empty;
            IsFolder = AssetDatabase.IsValidFolder( assetPath );
        }
    }
}