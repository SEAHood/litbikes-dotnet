﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitBikes.Events;
using LitBikes.Game.Controller;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LitBikes.Server
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
            services.AddMvc();
            services.AddSignalR();

            var clientEventReceiver = new ClientEventReceiver();
            var serverEventSender = new ServerEventSender();
            services.AddSingleton(serverEventSender);
            services.AddSingleton(clientEventReceiver);
            services.AddSingleton(new GameController(1, 1, clientEventReceiver, serverEventSender));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:1337")
                .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalHub>("/hub");
            });

        }
    }
}
