using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wcf.AttributeValidation
{
    public interface IValidator
    {
        IEnumerable<ValidationResult> Validate(IEnumerable<object> inputs, IServiceProvider serviceProvider);
    }
}