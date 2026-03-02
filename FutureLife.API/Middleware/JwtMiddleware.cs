using FutureLife.API.Helpers;

namespace FutureLife.API.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtHelper _jwt;

    public JwtMiddleware(RequestDelegate next, JwtHelper jwt)
    {
        _next = next;
        _jwt = jwt;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var userId = _jwt.ValidateTokenAndGetUserId(token);
            if (userId.HasValue)
            {
                context.Items["UserId"] = userId.Value;
            }
        }

        await _next(context);
    }
}
