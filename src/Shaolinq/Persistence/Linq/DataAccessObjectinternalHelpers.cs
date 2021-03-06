// Copyright (c) 2007-2018 Thong Nguyen (tumtumtum@gmail.com)

using System;
using System.Linq;
using System.Linq.Expressions;
using Platform;

namespace Shaolinq.Persistence.Linq
{
	public static class DataAccessObjectInternalHelpers
	{
		public static P AddToCollection<P, C>(P parent, Func<P, RelatedDataAccessObjects<C>> getChildren, C child, int version)
			where P : DataAccessObject
			where C : DataAccessObject
		{
			if (parent == null)
			{
				return null;
			}

			getChildren(parent).Add(child, version);

			return parent;
		}

		public static T IncludeDirect<T, U>(this T obj, Expression<Func<T, U>> include)
		{
			throw new InvalidOperationException("This method should only be called from within a LINQ query");
		}

		public static Expression GetPropertyValueExpressionFromPredicatedDeflatedObject<T, U>(T obj, string propertyPath)
			where T : DataAccessObject
		{
			var pathComponents = propertyPath.Split('.');
			var parameter = Expression.Parameter(typeof(T));

			var propertyExpression = pathComponents.Aggregate<string, Expression>
			(
				parameter,
				(instance, name) => Expression.Property(instance, instance.Type.GetMostDerivedProperty(name))
			);

			var expression = obj.dataAccessModel.GetDataAccessObjects<T>()
				.Where((Expression<Func<T, bool>>)obj.GetAdvanced().DeflatedPredicate)
				.Select(Expression.Lambda<Func<T, U>>(propertyExpression, parameter)).Expression;

			return Expression.Call(null, TypeUtils.GetMethod(() => default(IQueryable<U>).First()), expression);
		}
	}
}