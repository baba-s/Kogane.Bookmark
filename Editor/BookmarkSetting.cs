using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kogane.Internal
{
    [FilePath( "UserSettings/Kogane/BookmarkSetting.asset", FilePathAttribute.Location.ProjectFolder )]
    internal sealed class BookmarkSetting :
        ScriptableSingleton<BookmarkSetting>,
        IEnumerable<Object>
    {
        [SerializeField] private List<Object> m_list = new();

        public Object this[ int index ] => m_list[ index ];

        public int Count => m_list.Count;

        public static Action OnChanged { get; set; }

        public void Save()
        {
            Save( true );
            OnChanged?.Invoke();
        }

        public void Add( Object asset )
        {
            m_list.Add( asset );
            Save();
        }

        public void Remove( Object asset )
        {
            m_list.Remove( asset );
            Save();
        }

        public IEnumerator<Object> GetEnumerator()
        {
            return ( ( IEnumerable<Object> )m_list ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}