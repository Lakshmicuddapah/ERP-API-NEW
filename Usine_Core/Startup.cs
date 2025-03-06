using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usine_Core
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
            services.AddControllersWithViews();
            services.AddCors(c => c.AddPolicy("COMPolicy",
               builder =>
               {
                   builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
               }

               ));
            services.AddMvc();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddControllers();
            services.AddAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            

            app.UseAuthorization();

          

            MvcOptions opt = new MvcOptions();
            opt.EnableEndpointRouting = false;
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("COMPolicy");
            app.UseAuthentication();

            app.UseMvc();

            app.UseDefaultFiles();
            app.UseStaticFiles();
          

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
