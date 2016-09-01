﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using EpubSharp.Format;

namespace EpubSharp
{
    public class EpubBook
    {
        internal const string AuthorsSeparator = ", ";

        /// <summary>
        /// Raw epub format structures. This is populated only when the instance is retrieved using EpubReader.Read()
        /// </summary>
        public EpubFormat Format { get; internal set; }

        public string Title => Format.Opf.Metadata.Titles.FirstOrDefault() ?? string.Empty;
        public List<string> Authors => Format.Opf.Metadata.Creators.Select(creator => creator.Text).ToList();
        public string Author => string.Join(AuthorsSeparator, Authors);
        public EpubResources Resources { get; internal set; }
        public EpubSpecialResources SpecialResources { get; internal set; }

        internal Lazy<Image> LazyCoverImage = null;
        public Image CoverImage => LazyCoverImage?.Value;

        public List<EpubChapter> TableOfContents { get; internal set; }

        public string ToPlainText()
        {
            var builder = new StringBuilder();
            foreach (var html in SpecialResources.HtmlInReadingOrder)
            {
                builder.Append(HtmlProcessor.GetContentAsPlainText(html.TextContent));
                builder.Append('\n');
            }
            return builder.ToString().Trim();
        }
    }
    
    public class EpubChapter
    {
        public string Title { get; set; }
        public string FileName { get; set; }
        public string Anchor { get; set; }
        public List<EpubChapter> SubChapters { get; set; } = new List<EpubChapter>();

        public override string ToString()
        {
            return $"Title: {Title}, Subchapter count: {SubChapters.Count}";
        }
    }

    public class EpubResources
    {
        public Dictionary<string, EpubTextContentFile> Html { get; internal set; } = new Dictionary<string, EpubTextContentFile>();
        public Dictionary<string, EpubTextContentFile> Css { get; internal set; } = new Dictionary<string, EpubTextContentFile>();
        public Dictionary<string, EpubByteContentFile> Images { get; internal set; } = new Dictionary<string, EpubByteContentFile>();
        public Dictionary<string, EpubByteContentFile> Fonts { get; internal set; } = new Dictionary<string, EpubByteContentFile>();
        public Dictionary<string, EpubContentFile> Other { get; internal set; } = new Dictionary<string, EpubContentFile>();
    }

    public class EpubSpecialResources
    {
        public EpubTextContentFile Ocf { get; internal set; }
        public EpubTextContentFile Opf { get; internal set; }
        public List<EpubTextContentFile> HtmlInReadingOrder { get; internal set; } = new List<EpubTextContentFile>();
    }

    public abstract class EpubContentFile
    {
        public string FileName { get; set; }
        public EpubContentType ContentType { get; set; }
        public string MimeType { get; set; }
        public byte[] Content { get; set; }
    }

    public class EpubByteContentFile : EpubContentFile { }
    
    public class EpubTextContentFile : EpubContentFile
    {
        public string TextContent => Encoding.UTF8.GetString(Content);
    }
}
