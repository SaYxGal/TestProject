using System.Net;

namespace TestProject.Models.Exception;

public record ExceptionResponse(HttpStatusCode StatusCode, string Description, string? InnerException = "");
