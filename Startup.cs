using System;
using ElasticsearchIntro.Dtos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;

namespace ElasticsearchIntro
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
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddMediatR(typeof(Startup).Assembly);

            AddElasticsearch(services);

            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseOpenApi();
            app.UseSwaggerUi3(settings => settings.DocExpansion = "list");
        }

        private void AddElasticsearch(IServiceCollection services)
        {
            var settings = new ConnectionSettings(new Uri(Configuration["Elasticsearch:Url"]))
                .DefaultIndex(Configuration["Elasticsearch:Index"])
                .DefaultMappingFor<CompanyInformationDto>(c => c
                    .PropertyName(f => f.FinancialStatements, "fsCombines")
                )
                .EnableDebugMode();

            var user = Configuration["Elasticsearch:UserName"];
            var pass = Configuration["Elasticsearch:Password"];
            if (!string.IsNullOrEmpty(user))
            {
                settings.BasicAuthentication(user, pass);
                settings.ServerCertificateValidationCallback((sender, cert, chain, errors) => true);
            }

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
