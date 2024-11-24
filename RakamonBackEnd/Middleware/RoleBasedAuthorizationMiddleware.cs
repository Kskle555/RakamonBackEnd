namespace RakamonBackEnd.Middleware
{
    public class RoleBasedAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Endpoint'ten rol bilgilerini al
            var endpoint = context.GetEndpoint();
            var roleAttribute = endpoint?.Metadata.GetMetadata<RoleAttribute>();

            if (roleAttribute != null)
            {
                // Kullanıcı oturumdan rol bilgisini al
                var userRole = context.Session.GetString("Role");

                // Yetkisiz erişim kontrolü
                if (string.IsNullOrEmpty(userRole) || !roleAttribute.Roles.Contains(userRole))
                {
                    context.Response.StatusCode = 403; // Forbidden
                    await context.Response.WriteAsync("Yetkiniz yok");
                    return;
                }
            }

            await _next(context);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RoleAttribute : Attribute
    {
        public string[] Roles { get; }

        public RoleAttribute(params string[] roles)
        {
            Roles = roles;
        }
    }

}
