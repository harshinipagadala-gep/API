using System.Diagnostics.CodeAnalysis;
using GEP.SMART.Configuration;
using GEP.SMART.Requisition.API.Controllers.Filters;
using GEP.SMART.Requisition.API.Middleware;
using GEP.SMART.Security.ClaimsManagerCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace GEP.SMART.Requisition.API
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            //var conn = configuration.GetValue<string>("ServiceConfiguration:ConfigSqlConn");
            //var conn = configuration.GetConnectionString("ConfigSqlConn");
            //MultiRegionConfig.InitMultiRegionConfig(conn);
            MultiRegionConfig.IsCloudService = false;
            MultiRegionConfig.InitMultiRegionConfig();
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterServices();

            services.AddMvc(options =>
                            {
                                options.Filters.Add(typeof(HandleUnexpectedErrorAttribute));
                            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddJsonOptions(options => options.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat);

            services.AddSmartAuthentication();
            services.AddSwaggerGen(g =>
                    g.SwaggerDoc("v1", new Info { Title = "Requisition 2.0 API", Version = "v1", Description = "This provide the list of Requisition 2.0 available API's" }));
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Requisition API V1");
                });

            app.UseAuthentication();
            app.UseReqMiddleWare();
            app.UseMvc();
            //loggerFactory.AddLog4Net();
            app.UseExceptionMiddleware();
            
        }
    }
}
