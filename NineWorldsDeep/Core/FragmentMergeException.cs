using System;
using System.Runtime.Serialization;

namespace NineWorldsDeep.Core
{
    [Serializable]
    internal class FragmentMergeException : Exception
    {
        public FragmentMergeException()
        {
        }

        public FragmentMergeException(string message) : base(message)
        {
        }

        public FragmentMergeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FragmentMergeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}