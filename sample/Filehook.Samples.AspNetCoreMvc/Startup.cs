using System.IO;
using Filehook.Proccessors.Image.Abstractions;
using Filehook.Samples.AspNetCoreMvc.Models;
using Filehook.Storages.FileSystem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

            services.AddFilehook(FileSystemConsts.FileSystemStorageName)
                .AddFileSystemStorage(options =>
                {
                    options.BasePath = "./wwwroot";
                    options.CdnUrl = "http://localhost:5000";
                })
                .AddMetadata(builder => {
                    builder.Entity<Article>(entity => {
                        entity.HasName("MyArticle");

                        entity.Property(x => x.CoverImageFileName)
                            .HasName("FileName")
                            .HasImageStyle(new ImageStyle("thumb", new ImageResizeOptions { Width = 310 }))
                            .HasImageStyle(new ImageStyle("retina_thumb", new ImageResizeOptions { Height = 220 }))
                            .HasImageStyle(new ImageStyle("iphone", new ImageResizeOptions { Width = 640, Height = 1136 }))
                            .HasImageStyle(new ImageStyle("ipad", new ImageResizeOptions { Width = 768, Height = 1024 }))
                            .HasImageStyle(new ImageStyle("ipad_retina", new ImageResizeOptions { Width = 1536, Height = 2048 }))
                            .HasImageStyle(new ImageStyle("desktop_hd", new ImageResizeOptions { Height = 720 }))
                            .HasImageStyle(new ImageStyle("desktop_full_hd", new ImageResizeOptions { Height = 1080, Mode = ImageResizeMode.ShrinkLarger }))
                            .HasImageStyle(new ImageStyle("desktop_retina", new ImageResizeOptions { Height = 1080, Mode = ImageResizeMode.ShrinkLarger }));

                        entity.Property(x => x.AttachmentFileName)
                            .HasName("Attachment");
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
