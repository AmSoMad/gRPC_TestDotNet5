using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace a03_gRPCTestCS
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {          
            services.AddGrpc(options =>
            {
                options.Interceptors.Add<GrpcServices.ServerLoggerInterceptor>();
            });
            //services.AddCors(); //추가함
            services.AddCors(o => o.AddPolicy("AllowAll",builder =>
            {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding"); 
            }));
            GrpcServices.Startup.ConfigureServices(services);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //app.UseGrpcWeb();//추가함
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true }); //추가함
            app.UseCors();//추가함

            app.UseEndpoints(endpoints =>
            {
                GrpcBindServer.GrpcBindService.UseEndpoints(endpoints);

                //endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb(); //추가함
                endpoints.MapGrpcService<GreeterService>(); 


                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
