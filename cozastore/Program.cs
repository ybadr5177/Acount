using AccountDAL.config;
using AccountDAL.Eentiti;
using AccountDAL.Repositories;
using AccountDAL.Services;
using BAL;
using BAL.Service;
using cozastore.Helpers;
using cozastore.Helpers.Resolver;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Globalization;
using System.Text;

namespace cozastore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<CozaStoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            //Redis services
            builder.Services.AddSingleton<IConnectionMultiplexer>(S =>
            {

                var connection = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"));
                connection.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(connection);
            });
            //var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
            //var options = ConfigurationOptions.Parse(redisConnectionString);
            //options.Ssl = true; // لأن Upstash يحتاج TLS
            //options.AbortOnConnectFail = false;

            //builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));

            //token

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddIdentityCore<AppUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<InstagramService>();
            builder.Services.AddHttpClient();
            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.AddScoped<SliderLinkResolver>();
            builder.Services.AddScoped<SliderImageResolver>();
            builder.Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddLocalization();
            builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            builder.Services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider=(type,factory)=>factory.Create(typeof(JsonStringLocalizerFactory));

                });
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCulturres = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")


                };
                options.DefaultRequestCulture = new RequestCulture(culture: supportedCulturres[0], uiCulture: supportedCulturres[0]);
                options.SupportedCultures = supportedCulturres;
                options.SupportedUICultures= supportedCulturres;
            });


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // قراءة التوكن من الكوكيز
                            var token = context.Request.Cookies["AuthToken"];
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JWT:ValidAudience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                        ValidateLifetime = true,

                        // ✅ إضافة هذا السطر علشان لو عامل انتحراشن من api خرجي اقدر اجيب اسم المستخدم:
                        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"

                    };
                });
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // مدة صلاحية السيشن
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IMessageService, MessageService>();



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

            var SupportedCulturres = new[] { "en", "ar" };
            var localizationOptions= new RequestLocalizationOptions()
                .SetDefaultCulture(SupportedCulturres[0])
                .AddSupportedCultures(SupportedCulturres)
                .AddSupportedUICultures(SupportedCulturres);

            app.UseRequestLocalization(localizationOptions);
            app.UseStatusCodePages(context =>
            {
                var response = context.HttpContext.Response;

                if (response.StatusCode == 401 || response.StatusCode == 403)
                {
                    response.Redirect("/Account/Login");
                }

                return Task.CompletedTask;
            });
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();



         
            app.MapControllerRoute(

                name: "default",
                pattern: "{controller=Mastar}/{action=index}/{id?}");

            app.Run();
        }
    }
}