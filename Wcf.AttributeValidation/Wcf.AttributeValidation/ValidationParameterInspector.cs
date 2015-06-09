using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace Wcf.AttributeValidation
{
    public class ValidationParameterInspector<T> : IParameterInspector
        where T: ValidationError, new()
    {
        private readonly Func<IValidator> _validatorFactory;
        private readonly IServiceProvider _serviceProvider;

        public ValidationParameterInspector(Func<IValidator> validatorFactory, IServiceProvider serviceProvider)
        {
            _validatorFactory = validatorFactory;
            _serviceProvider = serviceProvider;
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            Validate(operationName, inputs, _serviceProvider);
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
        }

        private void Validate(string operationName, IEnumerable<object> inputs, IServiceProvider serviceProvider)
        {
            var validator = _validatorFactory();
            var validationResults = validator.Validate(inputs, serviceProvider)
                .Select(c => new T{Message = c.ErrorMessage, PropertyNames = c.MemberNames.ToArray()})
                .ToArray();
            if (validationResults.Any())
                throw new FaultException<T[]>(validationResults, new FaultReason("ValidationFailed"));
        }
    }

    public class ValidationParameterInspector : ValidationParameterInspector<ValidationError>
    {
        public ValidationParameterInspector(Func<IValidator> validatorFactory, IServiceProvider serviceProvider)
            : base(validatorFactory, serviceProvider)
        { }

    }
}