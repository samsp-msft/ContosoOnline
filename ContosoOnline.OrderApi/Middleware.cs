namespace ContosoOnline.OrderApi;

public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    private int requestCount = 0;
    private Random random = new Random();

    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (requestCount++ > 3)
        {
            if (random.Next(100) > 70) { Task.Delay(random.Next(2000) * requestCount).Wait(); }
        }
        await _next(context);
    }

}

public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomMiddleware>();
    }
}
