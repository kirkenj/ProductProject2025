using System.Net;
using Microsoft.AspNetCore.Mvc;


namespace CustomResponse
{
    public class Response<T>
    {
        public string Message { get; set; } = string.Empty;
        public T? Result { get; set; } = default!;
        public HttpStatusCode StatusCode { get; set; } = default!;
        public bool Success => (int)StatusCode - 200 < 100;


        public static Response<T> BadRequestResponse(string message) => new()
        {
            Message = message,
            StatusCode = HttpStatusCode.BadRequest,
            Result = default
        };

        public static Response<T> ServerErrorResponse(string message) => new()
        {
            Message = message,
            StatusCode = HttpStatusCode.InternalServerError,
            Result = default
        };

        public static Response<T> NotFoundResponse(string message, bool messageIsArgumentName = false) => new()
        {
            Message = messageIsArgumentName ? $"Object with given {message} not found" : message,
            StatusCode = HttpStatusCode.NotFound,
            Result = default
        };

        public static Response<T> OkResponse(T result, string message) => new()
        {
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
