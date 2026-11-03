using EyeCare.Application.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EyeCare.Infrastructure.Persistence;

public static class DatabaseExceptionHandler
{
    private const string AppointmentSlotIndex =
        "UX_Appointments_DoctorId_StartAt_Active";

    public static void Handle(DbUpdateException exception)
    {
        if (exception.InnerException is not SqlException sqlException)
        {
            throw exception;
        }

        if (exception is DbUpdateException ConcurrencyException)
        {
            if (IsUniqueConstraintViolation(
                    ConcurrencyException,
                    AppointmentSlotIndex))
            {
                throw new ConcurrencyException(
                "The doctor already has an appointment for this time slot.");
            }
        }

        if (exception is DbUpdateException dbUpdateException)
        {
            if (IsUniqueConstraintViolation(
                    dbUpdateException,
                    "UX_Payments_IdempotencyKey"))
            {
                throw new IdempotencyConflictException();
            }
            if (IsUniqueConstraintViolation(
          dbUpdateException,
          "UX_Invoices_AppointmentId"))
            {
                throw new InvoiceAlreadyExistsException();
            }
            if (IsUniqueConstraintViolation(
          dbUpdateException,
          "UX_PaymentWebhookEvents_EventId"))
            {
                throw new IdempotencyConflictException();
            }
            if (IsUniqueConstraintViolation(
        dbUpdateException,
        "UX_Refunds_IdempotencyKey"))
            {
                throw new IdempotencyConflictException();
            }
        }


        if (exception is DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException(
                "The resource was modified by another request. Please reload and try again.");
        }
       

        throw exception;
    }


    private static bool IsUniqueConstraintViolation(
       DbUpdateException exception,
       string constraintName)
    {
        if (exception.InnerException is not Microsoft.Data.SqlClient.SqlException sqlException)
            return false;

        return (sqlException.Number == 2601 ||
                sqlException.Number == 2627)
               && sqlException.Message.Contains(
                   constraintName,
                   StringComparison.OrdinalIgnoreCase);
    }
}
