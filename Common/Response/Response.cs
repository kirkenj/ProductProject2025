using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace CustomResponse
{
    public class Response<T>
    {
        public string Message { get; set; } = string.Empty;
        public T? Result { get; set; } = default!;
        public HttpStatusCode StatusCode { get; set; } = default!;
        public bool Success { get; set; } = default!;


        public static Response<T> BadRequestResponse(string message) => new()
        {
            Message = message,
            Success = false,
            StatusCode = HttpStatusCode.BadRequest,
            Result = default
        };

        public static Response<T> ServerErrorResponse(string message) => new()
        {
            Message = message,
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError,
            Result = default
        };

        public static Response<T> NotFoundResponse(string message, bool messageIsArgumentName = false) => new()
        {
            Success = false,
            Message = messageIsArgumentName ? $"Object with given {message} not found" : message,
            StatusCode = HttpStatusCode.NotFound,
            Result = default
        };

        public static Response<T> OkResponse(T result, string message) => new()
        {
            Success = true,
            Message = message,
            Result = result,
            StatusCode = HttpStatusCode.OK
        };

        public ActionResult GetActionResult() =>
        new ObjectResult(Success ? Result : Message)
        {
            StatusCode = (int)StatusCode
        };
    }
}
