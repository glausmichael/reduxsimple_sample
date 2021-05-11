using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ReduxSimple.Common
{
    public static class PartialCopyExtensions
    {
        public static T CopyWith<T>(this T @object, Expression<Func<T>> partialCopyExpression)
            where T : class
        {
            if (!(partialCopyExpression.Body is MemberInitExpression memberInitExpression))
            {
                throw new InvalidOperationException($"The partial copy expression must be a MemberInitExpression -> new {typeof(T).Name} {{ .. }}");
            }

            var partialCopiedObject = partialCopyExpression.Compile().Invoke();
            var targetType = @object.GetType();

            var boundProperties = new HashSet<string>(memberInitExpression.Bindings.Select(b => b.Member.Name));
            foreach (var propertyInfo in targetType.GetProperties())
            {
                if (propertyInfo.CanRead && propertyInfo.CanWrite && !boundProperties.Contains(propertyInfo.Name))
                {
                    propertyInfo.SetValue(partialCopiedObject, propertyInfo.GetValue(@object));
                }
            }

            return partialCopiedObject;
        }
    }
}