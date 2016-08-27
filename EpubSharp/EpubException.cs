﻿using System;

namespace EpubSharp
{
    public class EpubException : Exception
    {
        public EpubException(string message) : base(message) { }
    }

    public class EpubParseException : EpubException
    {
        public EpubParseException(string message) : base($"EPUB parsing error: {message}") { }
    }
}
