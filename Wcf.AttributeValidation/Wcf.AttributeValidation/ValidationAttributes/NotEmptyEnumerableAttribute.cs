using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Wcf.AttributeValidation.ValidationAttributes
{
    public class NotEmptyEnumerableAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var collection = value as IEnumerable;
            return collection == null || collection.Cast<object>().Any();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsValid(value)) return ValidationResult.Success;
            string[] memberNames;

            if (validationContext == null || validationContext.MemberName == null)
                memberNames = new string[0];
            else
            {
                memberNames = new string[1]
                {
                    validationContext.MemberName
                };
            }
            var result = new ValidationResult("Collection is empty",
                memberNames);
            return result;
        }
    }
}
