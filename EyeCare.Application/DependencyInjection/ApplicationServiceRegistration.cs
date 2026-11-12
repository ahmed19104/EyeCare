using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using EyeCare.Application.Common.Behaviors;

namespace EyeCare.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly =
            typeof(ApplicationServiceRegistration).Assembly;

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(TracingBehavior<,>));

        return services;
    }
}