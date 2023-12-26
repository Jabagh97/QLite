using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace PortalPOC.Helpers
{
    public static class QueriesHelper
    {

        public static IQueryable DynamicJoin(
         IQueryable outer,
         Type outerType,
         IQueryable inner,
         Type innerType,
         string outerKeySelector,
         string innerKeySelector,
         string resultSelector)
        {

            var outerParam = Expression.Parameter(outerType, "outer");
            var innerParam = Expression.Parameter(innerType, "inner");

            var outerKey = DynamicExpressionParser.ParseLambda(outerType, null, outerKeySelector);
            var innerKey = DynamicExpressionParser.ParseLambda(innerType, null, innerKeySelector);

            var outerKeyLambda = Expression.Lambda(outerKey.Body, outerParam);
            var innerKeyLambda = Expression.Lambda(innerKey.Body, innerParam);

            var resultSelectorLambda = DynamicExpressionParser.ParseLambda(outerType, innerType, resultSelector);

            var joinExpression = Expression.Call(
                typeof(Queryable),
                "Join",
                new[] { outerType, innerType, outerKeyLambda.Body.Type, resultSelectorLambda.Body.Type },
                outer.Expression,
                inner.Expression,
                Expression.Quote(outerKeyLambda),
                Expression.Quote(innerKeyLambda),
                Expression.Quote(resultSelectorLambda)
            );

            return outer.Provider.CreateQuery(joinExpression);
        }
    }
}
