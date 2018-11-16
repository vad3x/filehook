using System.IO;
using System.Reflection;
using Filehook.Proccessors.Image.Abstractions;
using Filehook.Samples.AspNetCoreMvc.Infrastructure;
using Filehook.Samples.AspNetCoreMvc.Models;
using Filehook.Storages.FileSystem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddUserSecrets<Startup>();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ExampleDbContext>(
                o => o.UseMySql(Configuration.GetConnectionString("ExampleConnection")));

            services.AddFilehook(FileSystemConsts.FileSystemStorageName)
                .AddEntityFrameworkStores(x => x.UseMySql(Configuration.GetConnectionString("ExampleConnection"), o => o.MigrationsAssembly(migrationsAssembly)))
                .AddImageSharpImageProccessor()
                .AddFallbackFileProccessor()
                .AddFileSystemStorage(options =>
                {
                    options.BasePath = "./wwwroot";
                    options.HostUrl = "http://localhost:5000";
                })
                .AddMetadata(builder => {
                    builder.Entity<Article>(entity => {
                        entity.HasId(x => x.Id.ToString());
                        entity.HasName("MyArticle");
                    });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // map wwwroot
            app.UseStaticFiles();

            // map folders
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "./wwwroot/public")),
                RequestPath = new PathString("/public")
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
