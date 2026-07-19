using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebAPI.Exceptions;

namespace WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
            {
                _logger.LogError(pgEx, "Ошибка PostgreSQL");

                if (pgEx.SqlState == "23505")
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Type = "Conflict",
                        Message = "Файл с таким именем уже обрабатывается или существует"
                    });
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Type = "DatabaseError",
                        Message = "Ошибка базы данных"
                    });
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка обновления базы данных");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Type = "DatabaseError",
                    Message = "Произошла ошибка при сохранении данных"
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError(ex, "Объект не найден в базе данных");

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Type = "DatabaseError",
                    Exception = ex
                };

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанная ошибка");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    Type = "Internal Server Error",
                    Message = "Произошла внутренняя ошибка сервера"
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
