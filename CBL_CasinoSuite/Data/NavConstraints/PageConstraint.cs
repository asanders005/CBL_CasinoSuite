namespace CBL_CasinoSuite.Data.NavConstraints
{
    public class PageConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values[routeKey] == null)
            {
                return false;
            }

            if (int.TryParse(values[routeKey]?.ToString(), out int key) && key > 0)
            {
                return true;
            }

            if (values[routeKey].ToString().Length < 5)
            {
                return false;
            }

            string subRoute = values[routeKey].ToString().Substring(0, 4);
            if (subRoute.ToLower() == "page")
            {
                string pageNumber = values[routeKey].ToString().Substring(4);
                if (int.TryParse(pageNumber, out int page) && page > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
