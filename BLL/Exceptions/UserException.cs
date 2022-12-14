using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace BLL.Exceptions
{
    [Serializable]
    public class UserException : Exception
    {
        public string Name { get; set; }

        public UserException()
            : base() { }

        public UserException(string message)
            : base(message)
        { }

        public UserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
