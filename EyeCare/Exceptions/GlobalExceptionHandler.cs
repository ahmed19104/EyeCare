using EyeCare.Application.Exceptions;
using EyeCare.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EyeCareAPI.Exceptions;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException
    or DomainException
    or NotFoundException
    or UnauthorizedException
    or ForbiddenException)
        {
            _logger.LogWarning(
                "Request failed with expected exception type {ExceptionType}.",
                exception.GetType().Name);
        }
        else if (exception is ConflictException
            or ConcurrencyException
            or IdempotencyKeyReuseException
            or IdempotencyConflictException)
        {
            _logger.LogWarning(
                "Request encountered a business conflict: {ExceptionType}.",
                exception.GetType().Name);
        }
        else
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred.");
        }

        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        switch (exception)
        {
            case ValidationException validationException:

                problemDetails.Status =
                    StatusCodes.Status400BadRequest;

                problemDetails.Title =
                    "Validation failed.";

                problemDetails.Detail =
                    "One or more validation errors occurred.";

                problemDetails.Extensions["errors"] =
                    validationException.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group
                                .Select(error => error.ErrorMessage)
                                .ToArray());

                break;

            case DomainException domainException:

                problemDetails.Status =
                    StatusCodes.Status400BadRequest;

                problemDetails.Title =
                    "Business rule violation.";

                problemDetails.Detail =
                    domainException.Message;

                break;

            case NotFoundException notFoundException:

                problemDetails.Status =
                    StatusCodes.Status404NotFound;

                problemDetails.Title =
                    "Resource not found.";

                problemDetails.Detail =
                    notFoundException.Message;

                break;

            case ConflictException conflictException:

                problemDetails.Status =
                    StatusCodes.Status409Conflict;

                problemDetails.Title =
                    "Conflict.";

                problemDetails.Detail =
                    conflictException.Message;

                break;
            
            case ConcurrencyException concurrencyException:

                problemDetails.Status =
                    StatusCodes.Status409Conflict;

                problemDetails.Title =
                    "Concurrency conflict.";

                problemDetails.Detail =
                    concurrencyException.Message;

                break;

            case UnauthorizedException unauthorizedEx:
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Detail = unauthorizedEx.Message;
             

                break;

            case ForbiddenException forbiddenException:
                problemDetails.Status = StatusCodes.Status403Forbidden;
                problemDetails.Title = "Forbidden";
                problemDetails.Detail = forbiddenException.Message;
                break;

            case IdempotencyKeyReuseException idempotencyKeyReuseEx:
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "Idempotency key reuse error.";
                problemDetails.Detail = idempotencyKeyReuseEx.Message;

                _logger.LogWarning(exception, "Idempotency key reuse exception occurred.");
                break;

            case IdempotencyConflictException idempotencyConflictEx:
                problemDetails.Status = StatusCodes.Status409Conflict;
                problemDetails.Title = "Idempotency conflict.";
                problemDetails.Detail = idempotencyConflictEx.Message;

                _logger.LogWarning(exception, "Idempotency conflict occurred.");
                break;

            case PaymentProcessingException paymentProcessingException:

                problemDetails.Status =
                    StatusCodes.Status500InternalServerError;

                problemDetails.Title =
                    "Payment processing requires reconciliation.";

                problemDetails.Detail =
                    paymentProcessingException.Message;

                break;

            case InvoiceAlreadyExistsException invoiceAlreadyExistsException:

                problemDetails.Status =
                    StatusCodes.Status409Conflict;

                problemDetails.Title =
                    "Invoice already exists.";

                problemDetails.Detail =
                    invoiceAlreadyExistsException.Message;

                break;

            default:

                problemDetails.Status =
                    StatusCodes.Status500InternalServerError;

                problemDetails.Title =
                    "An unexpected error occurred.";

                problemDetails.Detail =
                    "An unexpected error occurred while processing the request.";

                break;
        }

        httpContext.Response.StatusCode =
            problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}
