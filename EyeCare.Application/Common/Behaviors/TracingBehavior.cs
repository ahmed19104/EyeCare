
using System.Diagnostics;
using MediatR;

namespace EyeCare.Application.Common.Behaviors;

public sealed class TracingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly ActivitySource ActivitySource =
        new("EyeCare.Application");

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(
            typeof(TRequest).Name,
            ActivityKind.Internal);

        activity?.SetTag(
            "mediatr.request.type",
            typeof(TRequest).Name);

        try
        {
            var response = await next();

            activity?.SetStatus(ActivityStatusCode.Ok);

            return response;
        }
        catch (Exception exception)
        {
            activity?.SetTag(
                "error.type",
                exception.GetType().FullName);

            activity?.SetStatus(ActivityStatusCode.Error);

            throw;
        }
    }
}
