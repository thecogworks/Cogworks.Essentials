using System.Security.Principal;

namespace Cogworks.Essentials.Extensions
{
    public static class RolePrincipalExtensions
    {
        public static bool IsUserLoggedIn(this IPrincipal user)
            => user.HasValue() && user.Identity.HasValue() && user.Identity.Name.HasValue();

        public static string GetUser(this IPrincipal user)
            => user?.Identity?.Name;
    }
}
