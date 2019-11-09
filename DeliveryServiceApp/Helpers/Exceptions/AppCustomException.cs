using System;
using System.Net;

namespace DeliveryServiceApp.Helpers.Exceptions
{
    /// <summary>Class that defines the Application Custom Exception.</summary>
    /// <seealso cref="System.Exception" />
    public class AppCustomException : Exception
    {
        public int? StatusCode { get; set; }

        public AppCustomException() : base() { }

        public AppCustomException(int errorCode, string message)
            : base(ComposeErrorMessage(errorCode, message))
        {
            StatusCode = (int?)HttpStatusCode.InternalServerError;
        }

        public AppCustomException(int errorCode, string message, int httpStatusCode)
            : base(ComposeErrorMessage(errorCode, message))
        {
            StatusCode = httpStatusCode;
        }


        #region Private methods

        /// <summary>Composes the error message.</summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <returns>The application error message.</returns>
        private static string ComposeErrorMessage(int errorCode, string message)
        {
            return "APP ERROR " + errorCode + ": " + message;
        }

        #endregion
    }
}