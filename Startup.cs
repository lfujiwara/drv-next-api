using System;
using System.Globalization;
using System.Security.Claims;
using drv_next_api.Data;
using drv_next_api.QueryServices.Trips;
using drv_next_api.Services;
using drv_next_api.Services.Customers;
using drv_next_api.Services.Trips;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace drv_next_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MapperProfile));
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                    DateTimeStyles = DateTimeStyles.AdjustToUniversal
                });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "drv_next_api", Version = "v1"});
            });
            services.AddDbContext<ApplicationContext>(opts =>
            {
                opts.UseSqlServer(Configuration.GetValue<string>("ASPNETCORE_CONNECTION_STRING") ??
                                  Environment.GetEnvironmentVariable("ASPNETCORE_CONNECTION_STRING"));
                // opts.UseInMemoryDatabase("Default");
            });
            services.AddTransient<ApplicationContext>();
            services.AddTransient<CustomersService>();
            services.AddTransient<TripsService>();
            services.AddTransient<TripsQueryService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = Environment.GetEnvironmentVariable("ASPNETCORE_AUTH0_AUTHORITY");
                options.Audience = Environment.GetEnvironmentVariable("ASPNETCORE_AUTH0_AUDIENCE");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "drv_next_api v1"));
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            if (Environment.GetEnvironmentVariable("ASPNETCORE_CORS") != null)
                app.UseCors(builder => builder.WithOrigins(Environment.GetEnvironmentVariable("ASPNETCORE_CORS"))
                    .AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(p => true));

            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}