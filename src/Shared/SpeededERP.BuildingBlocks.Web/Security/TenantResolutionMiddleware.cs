using SpeededERP.BuildingBlocks.Application.Security;

namespace SpeededERP.BuildingBlocks.Web.Security;

/// <summary>
/// Resolves the tenant for pre-authentication requests from the <c>X-Tenant-Slug</c>
/// header, stashing it on <see cref="HttpContext.Items"/> for
/// <see cref="HttpTenantContext"/> to pick up (architecture section 17). The JWT
/// <c>tenant_id</c> claim still takes priority once a caller is authenticated —
/// <see cref="HttpTenantContext"/> checks it first — so this only ever matters for
/// anonymous endpoints like registering a brand-new tenant's first user.
/// </summary>
public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Tenant-Slug";

    public async Task InvokeAsync(HttpContext context, ITenantSlugResolver resolver)
    {
        var slug = context.Request.Headers[HeaderName].ToString();
        if (!string.IsNullOrWhiteSpace(slug))
        {
            var resolved = await resolver.ResolveAsync(slug, context.RequestAborted).ConfigureAwait(false);
            if (resolved is not null)
            {
                context.Items["ResolvedTenantId"] = resolved.TenantId;
                context.Items["TenantIsolationMode"] = resolved.IsolationMode;
            }
        }

        await next(context).ConfigureAwait(false);
    }
}

public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseSpeededErpTenantResolution(this IApplicationBuilder app) =>
        app.UseMiddleware<TenantResolutionMiddleware>();
}
