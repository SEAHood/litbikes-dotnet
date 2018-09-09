using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitBikes.Events;
using LitBikes.Game.Controller;
using LitBikes.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
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
            
            services.AddSingleton(new GameSettings
            {
                ArenaSize = Configuration.GetValue<int>("ArenaSize"),
                MinPlayers = Configuration.GetValue<int>("MinPlayers"),
                RoundDuration = Configuration.GetValue<int>("RoundDuration"),
                RoundCountdownDuration = Configuration.GetValue<int>("RoundCountdownDuration"),
            });

            services.AddSingleton<IClientEventReceiver, ClientEventReceiver>();
            services.AddSingleton<IServerEventSender, ServerEventSender>();
            services.AddSingleton<SendEventManager>();
            services.AddSingleton<GameController>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
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


            ActivatorUtilities.CreateInstance<SendEventManager>(provider);
            ActivatorUtilities.CreateInstance<GameController>(provider);
        }
        
    }
}
