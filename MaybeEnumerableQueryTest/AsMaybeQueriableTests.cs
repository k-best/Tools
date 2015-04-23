using System;
using System.Linq;
using ExpressionHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MaybeEnumerableQueryTest
{
    [TestClass]
    public class AsMaybequeryableTests
    {
        [TestMethod]
        public void ShouldWork()
        {
            var sequence = Enumerable.Range(0, 20000);
            var queryable = sequence.AsMaybeQueryable();
            queryable = queryable.Where(c => c > 9999).Select(c => c + 10000);
            var result = queryable.ToArray();
            Assert.AreEqual(10000, result.Length);
            Assert.AreEqual(20000, result[0]);
        }

        [TestMethod]
        public void ShouldCorrectlyAddMaybeMonad()
        {
            var sequence = new Stab0[100];
            for (int i = 0; i < 100; i++)
            {
                var st = new Stab0();
                if(i%2==0)
                    st.Property = new Stab1{Property = new Stab2{Name = i.ToString()}};
                if(i%3==0)
                    st.Property = new Stab1();
                sequence[i] = st;
            }
            var queryable = sequence.AsMaybeQueryable();
            var result = queryable.Select(c => c.Property.Property.Name);
            foreach (var name in result)
            {
                Console.WriteLine(name);
            }
        }

        private class Stab0
        {
            public Stab1 Property { get; set; }
        }

        private class Stab1
        {
            public Stab2 Property { get; set; }
        }

        private class Stab2
        {
            public string Name { get; set; }
        }
    }
}
