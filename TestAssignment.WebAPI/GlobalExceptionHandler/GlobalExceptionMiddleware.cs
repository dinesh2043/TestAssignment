using Microsoft.AspNetCore.Builder;

namespace TestAssignment.WebAPI.GlobalExceptionHandler
{
    public static class GlobalExceptionMiddleware
    {
        public static void UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
