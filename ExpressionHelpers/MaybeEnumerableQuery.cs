using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionHelpers
{
    public abstract class MaybeEnumerableQuery
    {
        internal static IQueryable Create(Type elementType, IEnumerable sequence)
        {
            Type seqType = typeof(EnumerableQuery<>).MakeGenericType(elementType);
            return (IQueryable)Activator.CreateInstance(seqType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { sequence }, null);
        }

        internal static IQueryable Create(Type elementType, Expression expression)
        {
            Type seqType = typeof(EnumerableQuery<>).MakeGenericType(elementType);
            return (IQueryable)Activator.CreateInstance(seqType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { expression }, null);
        }
    } 

    public class MaybeEnumerableQuery<T>: MaybeEnumerableQuery, IQueryProvider, IOrderedQueryable<T>
    {
        private readonly EnumerableQuery<T> _innerQuery;

        public MaybeEnumerableQuery(IEnumerable<T> enumerable)
        {
            _innerQuery = new EnumerableQuery<T>(enumerable);
        }

        public MaybeEnumerableQuery(Expression expression)
        {
            _innerQuery = new EnumerableQuery<T>(RewriteExpression(expression));
        }

        private Expression RewriteExpression(Expression expression)
        {
            var rewriter = new AddMaybeVisitor();
            return rewriter.Visit(expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return ((IQueryProvider)_innerQuery).CreateQuery(RewriteExpression(expression));
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return ((IQueryProvider)_innerQuery).CreateQuery<TElement>(RewriteExpression(expression));
        }

        public object Execute(Expression expression)
        {
            return ((IQueryProvider)_innerQuery).Execute(RewriteExpression(expression));
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return ((IQueryProvider)_innerQuery).Execute<TResult>(RewriteExpression(expression));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _innerQuery).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        Expression IQueryable.Expression
        {
            get { return ((IQueryable) _innerQuery).Expression; }
        }

        public Type ElementType
        {
            get { return ((IQueryable) _innerQuery).ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }
    }
}