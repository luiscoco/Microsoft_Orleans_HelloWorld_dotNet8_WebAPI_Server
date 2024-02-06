using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Microsoft.OpenApi.Models; // Necesario para Swagger
using System;
using System.Threading.Tasks;

try
{
    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureServices(services =>
            {
                // Añade servicios al contenedor.
                services.AddControllers();
                // Añade la herramienta de generación de Swagger
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });
                });
            });

            // Usa IWebHostEnvironment para acceder al entorno
            webBuilder.Configure((context, app) =>
            {
                var env = context.HostingEnvironment;

                // Configura el pipeline de solicitudes HTTP.
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwagger();
                    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API V1"));
                }

                app.UseRouting();

                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            });
        })
        .UseOrleans(siloBuilder =>
        {
            siloBuilder.UseLocalhostClustering()
                       .ConfigureLogging(logging => logging.AddConsole());
            // Configuración adicional de Orleans según sea necesario
        });

    using var host = builder.Build();
    Console.WriteLine("\n\nPresiona Enter para terminar...\n\n");
    await host.RunAsync();

    return;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return;
}
