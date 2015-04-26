using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionHelpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<TElement> AsMaybeQueryable<TElement>(this IEnumerable<TElement> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            var elements = source as IQueryable<TElement>;
            return elements ?? new MaybeEnumerableQuery<TElement>(source);
        }

        public static IQueryable AsMaybeQueryable(this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            var queryable = source as IQueryable;
            if (queryable != null)
                return queryable;
            var enumType = FindGenericType(typeof(IEnumerable<>), source.GetType());
            if (enumType == null)
                throw new ArgumentException("Source is not generic",nameof(source));
            return MaybeEnumerableQuery.Create(enumType.GetGenericArguments()[0], source);
        }

        private static Type FindGenericType(Type definition, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == definition)
                    return type;
                if (definition.IsInterface)
                {
                    foreach (Type itype in type.GetInterfaces())
                    {
                        Type found = FindGenericType(definition, itype);
                        if (found != null)
                            return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}