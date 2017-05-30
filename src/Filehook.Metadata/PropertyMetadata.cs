using System;
using System.Collections.Generic;
using Filehook.Abstractions;

namespace Filehook.Metadata
{
    public class PropertyMetadata
    {
        private List<FileStyle> _styles = new List<FileStyle>();

        public string Name { get; internal set; }

        public string StorageName { get; internal set; }

        public string Postfix { get; internal set; }

        public IEnumerable<FileStyle> Styles => _styles;

        internal void AddStyle(FileStyle style)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            _styles.Add(style);
        }
    }
}