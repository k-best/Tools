using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wcf.AttributeValidation.ValidationAttributes;

namespace Wcf.AttributeValidation.Test
{
    [TestClass]
    public class ObjectValidatorTest
    {
        [TestMethod]
        public void ShouldValidateObjectWithIEnumerable()
        {
            var objectToValidate = new ObjectWithIEnumerable()
            {
                Collection = new InnerObject []{new InnerObject(), new InnerObject{Value1 = "element"}, new InnerObject(), }
            };
            var validator = new ObjectValidator();

            var result = validator.Validate(new[] {objectToValidate}, null).ToArray();
            Assert.IsTrue(result.Any());
            Assert.AreEqual(2, result.Length);
        }
        [TestMethod]
        public void ShouldValidateIEnumerable()
        {
            var objectToValidate = GetEnumerator();
            var validator = new ObjectValidator();

            var result = validator.Validate(new[] { objectToValidate }, null).ToArray();
            Assert.IsTrue(result.Any());
            Assert.AreEqual(2, result.Length);
        }

        [TestMethod]
        public void ShouldValidateLargeArrays()
        {
            const int length = 1000000;
            var objectToValidate = new InnerObject[length];
            for (int i = 0; i < length; i++)
            {
                objectToValidate[i]=new InnerObject();
            }
            var validator = new ObjectValidator();

            var result = validator.Validate(new[] { objectToValidate }, null).ToArray();
            Assert.IsTrue(result.Any());
            Assert.AreEqual(length, result.Length);
        }

        private IEnumerable<InnerObject> GetEnumerator()
        {
            yield return new InnerObject{Value1="11"};
            yield return new InnerObject();
            yield return new InnerObject();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldNotThrowStackOverflowExceptionOnCircullarReference()
        {
            var objectToValidate = new ObjectWithCircularReference {Name = "object1"};
            objectToValidate.RelatedObject = objectToValidate;

            var validator = new ObjectValidator();

            var result = validator.Validate(new[] { objectToValidate }, null).ToArray();
        }
    }

    internal class ObjectWithCircularReference
    {
        public ObjectWithCircularReference RelatedObject { get; set; }

        public string Name { get; set; }
    }

    internal class ObjectWithIEnumerable
    {
        [NotEmptyEnumerable]
        public IEnumerable<InnerObject> Collection { get; set; } 
    }

    internal class InnerObject
    {
        [Required]
        public string Value1 { get; set; }
    }
}
