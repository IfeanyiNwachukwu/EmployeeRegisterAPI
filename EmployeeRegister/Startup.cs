using Contracts;
using EmployeeRegister.Extensions;
using EmployeeRegister.Filters.ActionFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System.IO;


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

            // ACTION FILTERS
            services.AddScoped<ValidationFilterAttribute>();  // Filter to do common model alidation in Post and Put requests
            services.AddScoped<ValidateCompanyExistsAttribute>();
            services.AddScoped<ValidateEmployeeForCompanyExists>();
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
            }).AddNewtonsoftJson()     //to enable patch requests    
               .AddXmlDataContractSerializerFormatters()
               .AddCustomCSVFormatter();
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
