using System.Runtime.Serialization;

namespace Wcf.AttributeValidation
{
    public class ValidationError
    {
        public ValidationError(string errorMessage, params string[] propertyNames)
        {
            Message = errorMessage;
            PropertyNames = propertyNames;
        }

        public ValidationError()
        {
        }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string[] PropertyNames { get; set; }
    }
}