// ---------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Standards.POC.Retry.Api.Brokers.DateTimes;
using Standards.POC.Retry.Api.Brokers.Loggings;
using Standards.POC.Retry.Api.Brokers.Storages;
using Standards.POC.Retry.Api.Models.Students;
using Standards.POC.Retry.Api.Services.Foundations.Students;

namespace Standards.POC.Retry.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddControllers().AddOData(options =>
            {
                options.EnableAttributeRouting = true;
                options.Select().Filter().Expand().OrderBy().Count().SetMaxTop(25);
                options.AddRouteComponents("odata", GetEdmModel());
            });

            services.AddDbContext<StorageBroker>();
            AddBrokers(services);
            AddServices(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Standards.POC.Retry.Api", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Standards.POC.Retry.Api v1"));
                app.UseODataRouteDebug();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<IStorageBroker, StorageBroker>();
        }

        private static void AddBrokers(IServiceCollection services)
        {
            services.AddTransient<IStudentService, StudentService>();
        }

        private IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Student>("Students");

            return builder.GetEdmModel();
        }
    }
}
