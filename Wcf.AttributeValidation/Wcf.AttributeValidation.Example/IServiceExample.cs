using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Wcf.AttributeValidation.ValidationAttributes;

namespace Wcf.AttributeValidation.Example
{
    [ServiceContract]
    public interface IServiceExample
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        [FaultContract(typeof(ValidationError[]))]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }
    
    [DataContract]
    public class CompositeType
    {

        [DataMember]
        [Required]
        public bool? BoolValue { get; set; }

        [DataMember]
        public string StringValue { get; set; }

        [DataMember]
        [NotEmptyEnumerable]
        public List<CollectionObjectType> CollectionValue { get; set; }
    }

    [DataContract]
    public class CollectionObjectType
    {
        [DataMember]
        [Required]
        public string RequiredStringValue { get; set; }
    }
}
