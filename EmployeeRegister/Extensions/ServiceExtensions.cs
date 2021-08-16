using AspNetCoreRateLimit;
using Contracts;
using EmployeeRegister.ContentNegotiation;
using EmployeeRegister.Controllers;
using Entities;
using LoggerServices;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeRegister.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Cross origin Resource Sharing(CORS) configuration
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                    
                });
            });
        /// <summary>
        /// IIS configuration
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureIISIntegration(this IServiceCollection services) =>
            services.Configure<IISOptions>(
                options =>
                {

                });
        /// <summary>
        /// Logger services
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureLoggerServices(this IServiceCollection services) =>
            services.AddScoped<ILoggerManager, LoggerManager>();
        /// <summary>
        /// Database setup/configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(
                opts =>
                opts.UseSqlServer(configuration.GetConnectionString("SqlConnection"),
                    b => b.MigrationsAssembly("EmployeeRegister"))); // Added a migration assembly because our entities are in a different project
        /// <summary>
        /// Repository Manager/ Unit of Work setup
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigurerepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        /// <summary>
        /// A custom csv formatter
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
            builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));
        /// <summary>
        /// For Adding Custom Media types/headers
        /// </summary>
        /// <param name="services"></param>
        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter
                    .SupportedMediaTypes.Add("application/vnd.employeeregister.hateoas+json");
                    newtonsoftJsonOutputFormatter
                    .SupportedMediaTypes.Add("application/vnd.employeeregister.apiroot+json");
                  
                }
                var xmlOutputFormatter = config.OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

            if (xmlOutputFormatter != null)
            {
                xmlOutputFormatter
                .SupportedMediaTypes.Add("application/vnd.employeeregister.hateoas+xml");
                xmlOutputFormatter
                .SupportedMediaTypes.Add("application/vnd.employeeregister.apiroot+xml");

                }
            });
        }
        /// <summary>
        /// Configuring/settig up API versioning
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;  // Adds API version to the header
                opt.AssumeDefaultVersionWhenUnspecified = true; // Assumes default version when unspecified
                opt.DefaultApiVersion = new ApiVersion(1, 0);// sets the default version count
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version"); // Allows passing the desired API version through the Header
                // Addig versioning coventions. With these the [ApiVersion} attribute can be removed
                opt.Conventions.Controller<CompaniesController>().HasApiVersion(new ApiVersion(1, 0));
                opt.Conventions.Controller<CompaniesV2Controller>().HasApiVersion(new ApiVersion(2, 0));

            }
                );
        }
        /// <summary>
        /// For caching responses
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureResponseCaching(this IServiceCollection services) => services.AddResponseCaching();

        /// <summary>
        /// For controlling cache Headers like cache-control, Expires, Etag and Last modified
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
            services.AddHttpCacheHeaders(
                (expirationOpt) =>
                {
                    expirationOpt.MaxAge = 65;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                },
                (validationOpt) =>
                {
                    validationOpt.MustRevalidate = true;
                }
                );
        /// <summary>
        /// Handles Rate Limiting/throttling
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>
            {
            new RateLimitRule
            {
            Endpoint = "*",
            Limit= 3,
            Period = "5m"
            }
            };
            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }
    }
}
