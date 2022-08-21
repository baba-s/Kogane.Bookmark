using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
    /// <summary>
    /// ブックマークのセーブデータを管理するクラス
    /// </summary>
    [FilePath( "UserSettings/Kogane/Bookmark.asset", FilePathAttribute.Location.ProjectFolder )]
    internal sealed class BookmarkSaveData : ScriptableSingleton<BookmarkSaveData>
    {
        //==============================================================================
        // 変数(SerializeField)
        //==============================================================================
        [SerializeField] private List<BookmarkData> m_list = new();

        //==============================================================================
        // プロパティ
        //==============================================================================
        [NotNull] public List<BookmarkData> List => m_list;

        //==============================================================================
        // 関数
        //==============================================================================
        /// <summary>
        /// 保存します
        /// </summary>
        public void Save()
        {
            Save( true );
        }

        /// <summary>
        /// 既に登録されているブックマークの場合 true を返します
        /// </summary>
        public bool Contains( string guid )
        {
            return m_list.Any( x => x.Guid == guid );
        }

        /// <summary>
        /// ブックマークを追加します
        /// </summary>
        public void Add( BookmarkData data )
        {
            m_list.Add( data );
        }

        /// <summary>
        /// ブックマークを削除します
        /// </summary>
        public void Remove( BookmarkData data )
        {
            var index = m_list.FindIndex( x => x.Guid == data.Guid );
            m_list.RemoveAt( index );
        }

        public bool Refresh()
        {
            var isRefresh = false;

            for ( var i = m_list.Count - 1; i >= 0; i-- )
            {
                var data = m_list[ i ];

                if ( !data.IsValid )
                {
                    m_list.RemoveAt( i );
                    isRefresh = true;
                }
            }

            if ( isRefresh )
            {
                Save();
            }

            return isRefresh;
        }
    }
}