using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Kogane.Internal
{
    /// <summary>
    /// ブックマークウィンドウのヘッダーを管理するクラス
    /// </summary>
    internal sealed class BookmarkHeader : MultiColumnHeader
    {
        //==============================================================================
        // 関数
        //==============================================================================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BookmarkHeader( MultiColumnHeaderState state ) : base( state )
        {
            const int buttonColumnWidth = 16;

            var columns = new MultiColumnHeaderState.Column[]
            {
                new()
                {
                    width               = buttonColumnWidth,
                    minWidth            = buttonColumnWidth,
                    maxWidth            = buttonColumnWidth,
                    headerContent       = new( "" ),
                    headerTextAlignment = TextAlignment.Center,
                },
                new()
                {
                    headerContent       = new( "Name" ),
                    headerTextAlignment = TextAlignment.Center,
                },
                new()
                {
                    width               = buttonColumnWidth,
                    minWidth            = buttonColumnWidth,
                    maxWidth            = buttonColumnWidth,
                    headerContent       = new( "" ),
                    headerTextAlignment = TextAlignment.Center,
                },
            };

            this.state = new( columns );
        }
    }
}