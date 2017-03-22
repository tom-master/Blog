﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NewBlogger.Application;
using NewBlogger.Application.Interface;
using NewBlogger.Model;
using NewBlogger.Repository;
using NewBlogger.Repository.Base;

namespace NewBlogger
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            

            // Add framework services.
            services.AddMvc();

            services.AddTransient<IBlogService, BlogService>();

            services.AddTransient<ICategoryService, CategoryService>();

            services.AddTransient<ICommentService, CommentService>();

            services.AddTransient<RepositoryBase<Blog>, MongodbRepository<Blog>>();

            services.AddTransient<RepositoryBase<Category>, MongodbRepository<Category>>();

            services.AddTransient<RepositoryBase<Comment>, MongodbRepository<Comment>>();

            services.AddTransient<RepositoryBase<Tag>, MongodbRepository<Tag>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
