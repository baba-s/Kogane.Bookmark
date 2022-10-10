using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
    internal static class TextureManager
    {
        private static readonly Dictionary<BookmarkColumnType, string> GUID_DICTIONARY = new()
        {
            { BookmarkColumnType.PING, "aa964a004f813ae4bbddd1f835a65e40" },
            { BookmarkColumnType.REMOVE, "960e1330c513734428bd2779e7bcec28" },
        };

        private static readonly Dictionary<BookmarkColumnType, Texture2D> TEXTURE_DICTIONARY = new();

        public static Texture2D Get( BookmarkColumnType buttonType )
        {
            if ( TEXTURE_DICTIONARY.TryGetValue( buttonType, out var result ) ) return result;

            var guid      = GUID_DICTIONARY[ buttonType ];
            var assetPath = AssetDatabase.GUIDToAssetPath( guid );
            var texture   = AssetDatabase.LoadAssetAtPath<Texture2D>( assetPath );

            TEXTURE_DICTIONARY[ buttonType ] = texture;

            return texture;
        }
    }
}