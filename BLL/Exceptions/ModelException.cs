using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BLL.Exceptions
{
    [Serializable]
    public class ModelException : Exception
    {
        public string Name { get; set; }

        public ModelException()
            : base() { }

        public ModelException(string message)
            : base(message)
        { }

        public ModelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ModelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
