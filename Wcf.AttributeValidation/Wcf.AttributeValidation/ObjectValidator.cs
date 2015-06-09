using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace Wcf.AttributeValidation
{
    public class ObjectValidator : IValidator
    {
        private int _safetyCounter;

        public IEnumerable<ValidationResult> Validate(IEnumerable<object> inputs, IServiceProvider serviceProvider)
        {
            return inputs.Aggregate((IEnumerable<ValidationResult>)new ValidationResult[0],
                (current, input) =>
                    current.Concat(ValidateObject(input, string.Empty, serviceProvider)));
        }

        private IEnumerable<ValidationResult> ValidateObject(object instance, string prefix, IServiceProvider serviceProvider)
        {
            if (instance == null)
                return new ValidationResult[0];
            var validationContext = new ValidationContext(instance, serviceProvider, null);
            return
                ValidateAttributes(instance, validationContext, prefix)
                    .Concat(ValidateIValidatableObject(instance, validationContext, prefix))
                    .Concat(
                        GetChildComplexObjects(instance)
                            .SelectMany(c => ValidateObject(c.Item1, prefix + c.Item2 + ".", serviceProvider)))
                    .ToArray();
        }

        private static IEnumerable<ValidationResult> ValidateAttributes(object instance,
            ValidationContext validationContext,
            string prefix)
        {
            PropertyDescriptor[] propertyDescriptorCollection =
                TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>().ToArray();

            IEnumerable<ValidationResult> valResults = propertyDescriptorCollection.SelectMany(
                prop => prop.Attributes.OfType<ValidationAttribute>(), (prop, attribute) =>
                {
                    validationContext.DisplayName = prop.Name;
                    ValidationResult validationresult = attribute.GetValidationResult(prop.GetValue(instance),
                        validationContext);
                    if (validationresult != null)
                    {
                        IEnumerable<string> memberNames = validationresult.MemberNames.Any()
                            ? validationresult.MemberNames.Select(c => prefix + c)
                            : new[] { prefix + prop.Name };
                        validationresult = new ValidationResult(validationresult.ErrorMessage, memberNames);
                    }
                    return validationresult;
                })
                .Where(
                    validationResult =>
                        validationResult !=
                        ValidationResult.Success);
            return
                valResults;
        }

        private static bool IsComplexType(PropertyDescriptor propertyDescriptor)
        {
            var converter = propertyDescriptor.Converter;
            return converter == null || !converter.CanConvertFrom(typeof(string));
        }

        private static IEnumerable<ValidationResult> ValidateIValidatableObject(object instance,
            ValidationContext validationContext,
            string prefix)
        {
            var validatableObject = instance as IValidatableObject;
            return validatableObject == null
                ? new ValidationResult[0]
                : validatableObject.Validate(validationContext)
                    .Select(
                        vr =>
                            new ValidationResult(vr.ErrorMessage,
                                vr.MemberNames.Select(mn => prefix + mn)));
        }

        private IEnumerable<Tuple<object, string>> GetChildComplexObjects(object instance)
        {
            Debug.Assert(instance != null);
            if (_safetyCounter > 100000)
                throw new InvalidOperationException("Possible circullar references");
            _safetyCounter++;
            //если массив, то у него нет комплексных дочерних свойств
            var enumerableInstance = instance as IEnumerable;
            if (enumerableInstance != null)
                foreach (var tuple in ValidateCollection(enumerableInstance, string.Empty)) yield return tuple;
            var propDescriptors =
                TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>().ToArray();
            foreach (var propertyDescriptor in propDescriptors)
            {
                var value = propertyDescriptor.GetValue(instance);
                if (value == null)
                    continue;
                var enumerableValue = value as IEnumerable;
                if (enumerableValue!=null)
                {
                    foreach (var tuple in ValidateCollection(enumerableValue, propertyDescriptor.Name)) yield return tuple;
                }
                if (IsComplexType(propertyDescriptor))
                    yield return Tuple.Create(value, propertyDescriptor.Name);
            }
        }

        private static IEnumerable<Tuple<object, string>> ValidateCollection(IEnumerable enumerableValue, string name)
        {
            var enumerator = 0;
            foreach (object element in enumerableValue)
            {
                yield return
                    Tuple.Create(element, string.Format("{0}[{1}]", name, enumerator));
                enumerator++;
            }
        }
    }
}