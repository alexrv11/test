using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Models.N.Core.Exceptions
{
    public class TechnicalException : Exception
    {
        public TechnicalException()
        {
        }

        public TechnicalException(string message) : base(message)
        {
        }

        public TechnicalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TechnicalException(string message, string technicalCode) : base(message)
        {
            this.TechnicalCode = technicalCode;
        }

        public TechnicalException(string message, Exception innerException, string technicalCode) : base(message, innerException)
        {
            this.TechnicalCode = technicalCode;
        }

        public string TechnicalCode { get; private set; }
    }
}
