using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DeliveryServiceApp.Helpers.Exceptions
{
    /// <summary>Middleware class to handle all the API exceptions.</summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        /// <summary>Invokes the API methods and handles any exception asynchronously.</summary>
        /// <param name="httpContext">The HTTP context.</param>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (AppCustomException ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }


        #region Private methods

        /// <summary>Handles the exception asynchronously.</summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The application custom exception.</param>
        /// <returns>The context with a response containing some details of the exception.</returns>
        private Task HandleExceptionAsync(HttpContext context, AppCustomException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode ?? (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails
            {
                Message = ex.Message,
                Source = ex.Source
            }.ToJson());
        }

        /// <summary>Handles the exception asynchronously.</summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>The context with a response containing some details of the exception.</returns>
        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails
            {
                Message = ex.Message,
                Source = ex.Source
            }.ToJson());
        }

        #endregion
    }
}
