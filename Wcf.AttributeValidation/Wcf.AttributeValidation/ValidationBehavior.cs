using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Wcf.AttributeValidation
{
    public class ValidationBehavior : IEndpointBehavior
    {
        private Func<IValidator> _validatorFactory;
        internal ValidationBehavior(bool enabled)
        {
            Enabled = enabled;
            _validatorFactory = ()=>new ObjectValidator();
        }

        public ValidationBehavior(Func<IValidator> validatorFactory, bool enabled)
        {
            _validatorFactory = validatorFactory;
            Enabled = enabled;
        }

        public bool Enabled { get; set; }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //If enable is not true in the config we do not apply the Parameter Inspector

            if (Enabled == false)
            {
                return;
            }

            foreach (var dispatchOperation in endpointDispatcher.DispatchRuntime.Operations)
            {

                dispatchOperation.ParameterInspectors.Add(
                    new ValidationParameterInspector(_validatorFactory, null));
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            if (Enabled == false)
            {
                return;
            }

            foreach (var clientOperation in clientRuntime.Operations)
            {
                clientOperation.ParameterInspectors.Add(
                    new ValidationParameterInspector(_validatorFactory, null));
            }
        }
    }
}