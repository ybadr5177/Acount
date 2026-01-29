using AccountDAL.config;
using AccountDAL.Eentiti;
using AccountDAL.Repositories;
using AccountDAL.Services;
using BAL;
using BAL.Service;
using DashBoard.Errors;
using DashBoard.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DashBoard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DashAppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDashConnection"));
            });
            builder.Services.AddDbContext<CozaStoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<DashAppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;

            }).AddEntityFrameworkStores<DashAppIdentityDbContext>();
            // ?? FIX START: ??? ????? ??????? ????? ????? ?????? (?? ????? 404/401)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    // ??? ??? ????? ?????? ??? ??? Hub? ???? 401 Unauthorized ????? ?? 302 Redirect
                    if (context.Request.Path.StartsWithSegments("/chathub") || context.Request.Path.StartsWithSegments("/Chat"))
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }

                    // ????? ???????? ???? ????? ??????? ??????????
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddIdentityCore<AppUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState.Where(M => M.Value.Errors.Count() > 0)
                                                         .SelectMany(M => M.Value.Errors)
                                                         .Select(E => E.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });
            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // 1. ????? ???? SignalR ????????
            builder.Services.AddSignalR();

            // 2. ????? CORS ?????? ???????? ?? ???? ?????? (cozastore)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("SignalRCorsPolicy",
                    builder => builder
                        // ?? ??? ????? ??? ??? ?????? ?????? ?????? cozastore
                        .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        // ?? ????? ?????? ?????? ??????? (????? ??????? DashBoard)
                        .AllowCredentials());
            });

            builder.Services.AddScoped<IMessageService, MessageService>();



            //builder.Services.AddAuthentication()
            // .AddCookie("DashIdentityScheme", options =>
            // {
            //     options.LoginPath = "/DashLogin";
            //     options.Cookie.Name = "DashIdentityCookie";
            // })
            // .AddCookie("AppUserIdentityScheme", options =>
            // {
            //     options.LoginPath = "/UserLogin";
            //     options.Cookie.Name = "AppUserIdentityCookie";
            // });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("SignalRCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<ChatHub>("/chathub");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=AccountDash}/{action=Login}/{id?}");
                

            app.Run();
        }
    }
}