using MyBook.Data.ViewModels;
using System.Net;

namespace MyBook.Exceptions
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;  
        }

        public async Task InvokeAsync(HttpContext httpContex)
        {
            try
            {
                await _next(httpContex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContex, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContex, Exception ex)
        {
            httpContex.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContex.Response.ContentType = "application/json";

            var response = new ErrorVM()
            {
                StatusCode = httpContex.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware",
                Path = "path-goes-here"
            };

            return httpContex.Response.WriteAsync(response.ToString());
        }
    }
}
