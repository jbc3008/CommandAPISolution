using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandAPI.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace CommandAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new MySqlConnectionStringBuilder();
            builder.ConnectionString = Configuration.GetConnectionString("MariaDBConnection");
            builder.UserID = Configuration["UserID"];
            builder.Password = Configuration["Password"];


            services.AddDbContext<CommandContext>(options => options.UseMySql
                (builder.ConnectionString, new MariaDbServerVersion(new Version(10,6,3))));

            // Registers services to enable the use of “Controllers” throughout our application
            services.AddControllers();

            // services.AddScoped<ICommandAPIRepo, MockCommandAPIRepo>();

            services.AddScoped<ICommandAPIRepo, SqlCommandAPIRepo>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // We “MapControllers” to our endpoints, i.e., we make use  of the Controller services (registered in the ConfigureServices method) as endpoints in the Request Pipeline.
                endpoints.MapControllers();
            });
        }
    }
}
