﻿using System.Linq.Expressions;
using Shaolinq.Persistence.Sql.Linq.Expressions;

namespace Shaolinq.Persistence.Sql.Linq
{
	public class SqlReferencesColumnDeferrabilityRemover
		: SqlExpressionVisitor
	{
		private SqlReferencesColumnDeferrabilityRemover()
		{	
		}

		protected override Expression VisitReferencesColumn(SqlReferencesColumnExpression referencesColumnExpression)
		{
			if (referencesColumnExpression.Deferrability != SqlColumnReferenceDeferrability.NotDeferrable)
			{
				return new SqlReferencesColumnExpression(referencesColumnExpression.ReferencedTableName, SqlColumnReferenceDeferrability.NotDeferrable, referencesColumnExpression.ReferencedColumnNames, referencesColumnExpression.OnDeleteAction, referencesColumnExpression.OnUpdateAction);
			}

			return base.VisitReferencesColumn(referencesColumnExpression);
		}

		public static Expression Remove(Expression expression)
		{
			return new SqlReferencesColumnDeferrabilityRemover().Visit(expression);
		}
	}
}
