using System;
using System.Runtime.Serialization;

namespace NineWorldsDeep.Studio
{
    public class InvalidNoteParseException : Exception
    {
        public InvalidNoteParseException()
            : base("InvalidNoteParseException")
        { }
    }
}