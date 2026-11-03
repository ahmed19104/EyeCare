using EyeCare.Application.Abstractions.Authorization;
using EyeCare.Application.Abstractions.Billing;
using EyeCare.Application.Abstractions.Identity;
using EyeCare.Application.Abstractions.Payments;
using EyeCare.Application.Abstractions.Persistence;
using EyeCare.Application.Abstractions.Security;
using EyeCare.Infrastructure.Authorization;
using EyeCare.Infrastructure.Authorization.Permissions;
using EyeCare.Infrastructure.Billing;
using EyeCare.Infrastructure.Caching;
using EyeCare.Infrastructure.Identity;
using EyeCare.Infrastructure.Payments;
using EyeCare.Infrastructure.Persistence;
using EyeCare.Infrastructure.Repositories;
using EyeCare.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EyeCare.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection"));
            });

            services.AddScoped(
                typeof(IRepository<>),
                typeof(Repository<>));

            services.AddScoped<IUnitOfWork>(
                provider => provider.GetRequiredService<AppDbContext>());

            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUser, CurrentUser>();
            
           services.AddScoped<IIdentityService, IdentityService>();

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.Lockout.MaxFailedAccessAttempts = 5;

                options.Lockout.AllowedForNewUsers = true;
            })
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager();

            services.Configure<JwtOptions>(
    configuration.GetSection(
        JwtOptions.SectionName));

            services.AddScoped<
                ITokenService,
                JwtTokenService>();


            services.AddAuthentication(
     JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options =>
     {
         var jwtOptions =
             configuration
                 .GetSection(JwtOptions.SectionName)
                 .Get<JwtOptions>()!;

         options.TokenValidationParameters =
             new TokenValidationParameters
             {
                 ValidateIssuer = true,
                 ValidIssuer = jwtOptions.Issuer,

                 ValidateAudience = true,
                 ValidAudience = jwtOptions.Audience,

                 ValidateIssuerSigningKey = true,
                 IssuerSigningKey =
                     new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(
                             jwtOptions.Key)),

                 ValidateLifetime = true,

                 ClockSkew = TimeSpan.Zero
             };
     });

            services.AddAuthorization();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<
    IUserBranchService,
    UserBranchService>();

            services.AddScoped<
    IBranchAuthorizationService,
    BranchAuthorizationService>();

            services.AddScoped<
    IPermissionService,
    PermissionService>();

            services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

            services.AddScoped<
                IAuthorizationHandler,
                PermissionAuthorizationHandler>();

            services.AddScoped<
    IResourceAuthorizationService,
    ResourceAuthorizationService>();

            services.AddSingleton<IPaymentGateway, FakePaymentGateway>();
            
            
            services.AddScoped<
    IPaymentWebhookSignatureValidator,
    FakePaymentWebhookSignatureValidator>();

            services.Configure<RefundOptions>(
    configuration.GetSection(
        RefundOptions.SectionName));

            services.AddScoped<IRefundPolicy, RefundPolicy>();

            services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            services.AddRedisCaching(configuration);

            services.Configure<RefreshTokenOptions>(
    configuration.GetSection("RefreshToken"));

            services.AddSingleton<IRefreshTokenSettings, RefreshTokenSettingsAdapter>();
           
            services.AddScoped<
      IRefreshTokenService,
      RefreshTokenService>();

            
            services.AddScoped<IRefreshTokenTokenProvider, RefreshTokenCryptography>();

            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
