using CBL_CasinoSuite.Data.Models;

namespace CBL_CasinoSuite.Data.NavConstraints
{
    public class LeaderboardFilterConstraint :IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values[routeKey] != null)
            {
                if (Enum.TryParse(typeof(EGameList), values[routeKey]?.ToString(), true, out var result) && Enum.IsDefined(typeof(EGameList), result))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
