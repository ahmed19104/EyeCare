using EyeCare.Application.DependencyInjection;
using EyeCare.Infrastructure.DependencyInjection;
using EyeCare.Infrastructure.Identity;
using EyeCareAPI.Exceptions;
using EyeCareAPI.Extensions;
using EyeCareAPI.Middleware;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.OpenApi;
using Serilog;
// «·‹ using «·’ÕÌÕ… ›Ì „ﬂ«‰Â« ›Êﬁ Œ«·’

namespace EyeCareAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();

            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                   /* .WriteTo.Console()*/;
            });

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

            // ≈÷«›… ≈⁄œ«œ«  Swagger „⁄ œ⁄„ «·‹ Authentication
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // 1.  ⁄—Ì› ‘ﬂ· «·ﬁ›· (“—«— «·‹ Authorize)
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "ﬁ„ »≈œŒ«· «· Êﬂ‰ „»«‘—… Â‰« (»œÊ‰ ﬂ·„… Bearer)",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                // 2.  ÿ»Ìﬁ «·Õ„«Ì… ⁄·Ï «·‹ Endpoints »«·ÿ—Ìﬁ… «·ÕœÌÀ…
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(
                builder.Configuration);
            var dataProtectionKeysPath =
             builder.Configuration["DataProtection:KeysPath"];

            var dataProtection = builder.Services
                .AddDataProtection()
                .SetApplicationName("EyeCare");

            if (!string.IsNullOrWhiteSpace(dataProtectionKeysPath))
            {
                dataProtection.PersistKeysToFileSystem(
                    new DirectoryInfo(dataProtectionKeysPath));
            }

            builder.Services.AddExceptionHandler<
                GlobalExceptionHandler>();
            builder.Services.AddCustomRateLimiting();
            builder.Services.AddProblemDetails();
            var app = builder.Build();
           await IdentitySeeder.SeedAsync(app.Services);
            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //  ›⁄Ì· Swagger
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // 1.  ”ÃÌ· „⁄—› «·ÿ·» √Ê·«
            app.UseMiddleware<RequestLoggingMiddleware>();

            // 2. «” Œ—«Ã «·‹ User ID Ê Ã„Ì⁄Â (ÌÃ» √‰ ÌﬂÊ‰ »⁄œ «·‹ Authentication Êﬁ»· «·‹ Serilog Logging)
            app.UseMiddleware<UserLoggingMiddleware>();

            // 3.  ›⁄Ì· Serilog Request Logging ·ÌﬁÊ„ »«· ﬁ«ÿ «·‹ RequestId Ê UserId „⁄«
            app.UseSerilogRequestLogging();

            app.UseRateLimiter();

            app.MapControllers();

            app.Run();
        }
    }
}