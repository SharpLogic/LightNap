using LightNap.Core.Api;
using LightNap.Core.Telemetry;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LightNap.WebApi.Middleware
{
    /// <summary>
    /// Middleware to handle exceptions globally.
    /// </summary>
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment, ITelemetryClient telemetryClient)
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions =
            new()
            {
                Converters = { new JsonStringEnumConverter() },
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

        /// <summary>
        /// Handles the HTTP request and catches exceptions.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
            }
            catch (UserFriendlyApiException ex)
            {
                if (ex.InnerException is not null)
                {
                    logger.LogError(ex, "User-friendly exception in Web API: {message}", ex.Message);
                }

                await ExceptionMiddleware.WriteErrorAsync(context,
                    new ApiResponseDto<string>()
                    {
                        ErrorMessages = ex.Errors,
                        Type = ApiResponseType.Error
                    });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in Web API: {message}", ex.Message);
                telemetryClient.TrackException(ex);

                // To simplify client-side development, we convert all remaining errors to easily digestible ApiResponseDtos.
                // We use string as the type because it doesn't matter since Result is going to be null anyway.
                ApiResponseDto<string> error;

                if (environment.IsDevelopment())
                {
                    if (string.IsNullOrWhiteSpace(ex.StackTrace))
                    {
                        error = new ApiResponseDto<string>()
                        {
                            Type = ApiResponseType.UnexpectedError,
                            ErrorMessages = [ex.Message]
                        };
                    }
                    else
                    {
                        error = new ApiResponseDto<string>()
                        {
                            Type = ApiResponseType.UnexpectedError,
                            ErrorMessages = [ex.Message, ex.StackTrace]
                        };
                    }
                }
                else
                {
                    error = new ApiResponseDto<string>()
                    {
                        Type = ApiResponseType.UnexpectedError,
                        ErrorMessages = ["Internal Server Error"]
                    };

                }

                await ExceptionMiddleware.WriteErrorAsync(context, error);
            }
        }

        /// <summary>
        /// Writes the error response as JSON.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="error">The error response to write.</param>
        /// <returns>A task that represents the completion of the write operation.</returns>
        private static async Task WriteErrorAsync(HttpContext context, ApiResponseDto<string> error)
        {
            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(error, ExceptionMiddleware._jsonSerializerOptions);
            await context.Response.WriteAsync(json);
        }
    }
}