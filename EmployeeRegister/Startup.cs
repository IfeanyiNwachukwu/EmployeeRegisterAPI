using Contracts;
using EmployeeRegister.Extensions;
using EmployeeRegister.Filters.ActionFilters;
using Entities.DataTransferObjects.ReadOnly;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Repository.DataShaper;
using System.IO;
using Contracts.DataShaper;
using EmployeeRegister.Utility;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;

namespace EmployeeRegister
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureIISIntegration();
            services.ConfigureLoggerServices();
            services.ConfigureSqlContext(Configuration);
            services.ConfigurerepositoryManager();
            services.AddAutoMapper(typeof(Startup));
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;  //Suppressing the 400 BadRequest() error
            });
            services.ConfigureVersioning();
            services.ConfigureResponseCaching();
            services.ConfigureHttpCacheHeaders();
            
            services.AddMemoryCache();

            // Rate Limiting and Throttling
            services.ConfigureRateLimitingOptions();
            services.AddHttpContextAccessor();
           


            // ACTION FILTERS
            services.AddScoped<ValidationFilterAttribute>();  // Filter to do common model alidation in Post and Put requests
            services.AddScoped<ValidateCompanyExistsAttribute>();
            services.AddScoped<ValidateEmployeeForCompanyExists>();
            services.AddScoped<IDataShaper<EmployeeDTO>, DataShaper<EmployeeDTO>>();

           
            services.AddScoped<ValidateMediaTypeAttribute>();
            services.AddScoped<EmployeeLinks>();
            /*   services.AddControllers();*/ //returns onlyJSON content by default


            //To allow XML content
            //services.AddControllers(config =>
            //{
            //    config.RespectBrowserAcceptHeader = true;
            //    config.ReturnHttpNotAcceptable = true; // Returns 406 Not Acceptable if client tries to negotiate for a media type server does not support
            //}).AddXmlDataContractSerializerFormatters();
         
            services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                // Using CacheProfile to apply the same caching rues to all resources
                config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
            }).AddNewtonsoftJson()
               .AddXmlDataContractSerializerFormatters()
               .AddCustomCSVFormatter();

           

            services.AddCustomMediaTypes();

            //to enable patch requests 
            //.AddCustomCSVFormatter();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.ConfigureExceptionHandler(logger);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();

            app.UseIpRateLimiting();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
