using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kogane.Internal
{
    /// <summary>
    /// ブックマークの項目を管理するクラス
    /// </summary>
    [Serializable]
    internal sealed class BookmarkData
        : TreeViewItem,
          ISerializationCallbackReceiver
    {
        //==============================================================================
        // 変数(SerializeField)
        //==============================================================================
        [SerializeField] private string m_guid;
        [SerializeField] private int    m_id;

        //==============================================================================
        // 変数
        //==============================================================================
        private Object m_asset;

        //==============================================================================
        // プロパティ
        //==============================================================================
        public string Guid => m_guid;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace( m_guid ) &&
            !string.IsNullOrWhiteSpace( AssetPath ) &&
            AssetDatabase.LoadAssetAtPath<Object>( AssetPath ) != null;

        public string Name => IsValid ? Asset.name : string.Empty;

        public Object Asset
        {
            get
            {
                if ( !IsValid ) return null;

                if ( m_asset == null )
                {
                    m_asset = AssetDatabase.LoadAssetAtPath<Object>( AssetPath );
                }

                return m_asset;
            }
        }

        public string AssetPath => AssetDatabase.GUIDToAssetPath( m_guid );
        public bool   IsFolder  => AssetDatabase.IsValidFolder( AssetPath );

        //==============================================================================
        // 関数
        //==============================================================================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BookmarkData( int id, string guid ) : base( id )
        {
            m_guid = guid;
        }

        /// <summary>
        /// シリアライズされる前に呼び出されます
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_id = id;
        }

        /// <summary>
        /// デシリアライズされた後に呼び出されます
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            id = m_id;
        }
    }
}