using System;
using System.Runtime.Serialization;

namespace NineWorldsDeep.Studio
{
    [Serializable]
    internal class InvalidScaleParseException : Exception
    {
        public InvalidScaleParseException()
        {
        }

        public InvalidScaleParseException(string message) : base(message)
        {
        }

        public InvalidScaleParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidScaleParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}