using System;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Wcf.AttributeValidation
{
    public class CustomBehaviorSection : BehaviorExtensionElement
    {
        private const string EnabledAttributeName = "enabled";
        private const string ValidatorTypeName = "type";

        [ConfigurationProperty(EnabledAttributeName, DefaultValue = true, IsRequired = false)]
        public bool Enabled
        {
            get { return (bool)base[EnabledAttributeName]; }
            set { base[EnabledAttributeName] = value; }
        }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty(ValidatorTypeName, IsRequired = true)]
        public Type ValidatorType
        {
            get
            {
                var validatorType = base[ValidatorTypeName] as Type;
                if(validatorType==null)
                    throw new ConfigurationErrorsException("Type of ValidatorType is not found or it does not implement IValidator interface");
                var interfaces = validatorType.GetInterfaces();
                if (interfaces == null || !interfaces.Contains(typeof(IValidator)))
                    throw new ConfigurationErrorsException("Type of ValidatorType is not found or it does not implement IValidator interface");
                return validatorType;
            }
        }

        protected override object CreateBehavior()
        {
            return new ValidationBehavior(this.Enabled);

        }

        public override Type BehaviorType
        {

            get { return typeof(ValidationBehavior); }


        }
    }
}
