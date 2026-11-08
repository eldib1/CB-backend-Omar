namespace ClutchBids.WebAPI.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new { message = "An internal server error occurred.", details = string.Empty };

        switch (exception)
        {
            case ArgumentNullException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new { message = "Invalid request data.", details = exception.Message };
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new { message = "Unauthorized access.", details = exception.Message };
                break;
            case KeyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new { message = "Resource not found.", details = exception.Message };
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new { message = "An internal server error occurred.", details = exception.Message };
                break;
        }

        // Log to console
        LogException(exception);

        // Log to file if enabled
        var enableFileLogging = _configuration.GetValue<bool>("Logging:EnableFileLogging");
        if (enableFileLogging)
        {
            LogToFile(exception);
        }

        return context.Response.WriteAsJsonAsync(response);
    }

    private void LogException(Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred. Message: {Message}", exception.Message);
    }

    private void LogToFile(Exception exception)
    {
        try
        {
            var logFilePath = _configuration.GetValue<string>("Logging:LogFilePath") ?? "logs/error.log";
            var logDirectory = Path.GetDirectoryName(logFilePath);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {exception.GetType().Name}: {exception.Message}\n{exception.StackTrace}\n";
            File.AppendAllText(logFilePath, logMessage);
        }
        catch
        {
            // Silently fail if file logging fails
        }
    }
}
