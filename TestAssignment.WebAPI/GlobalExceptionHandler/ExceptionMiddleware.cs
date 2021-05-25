using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace TestAssignment.WebAPI.GlobalExceptionHandler
{
    public class ExceptionMiddleware
    {
        //The _next parameter of RequestDeleagate type is a function delegate which can process our HTTP requests.
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        //to register our Ilogger and RequestDelegate through the dependency injection. 
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }
        //we need to create the Invoke() method because RequestDelegate can’t process requests without it.
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //If everything goes well, the _next delegate should process the request from our controller
                //and it should generate the successful response.
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                //But if a request is unsuccessful (and it is, because we are forcing exception), our 
                //middleware will trigger the catch block and call the HandleExceptionAsync method.
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new GlobalErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error."
            }.ToString());
        }
    }
}
