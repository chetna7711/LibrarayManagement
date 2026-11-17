namespace LibraryManagement.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("Request Started");

            Console.WriteLine(
                "Requested URL: " + context.Request.Path);

            // Pass request to next middleware
            await _next(context);

            Console.WriteLine("Response Completed");

            Console.WriteLine(
                "Status Code: " + context.Response.StatusCode);
        }
    }
}