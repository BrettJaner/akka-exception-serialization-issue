using System.Reflection;
using Akka.Cluster.Hosting;
using Akka.Hosting;
using Akka.Logger.Serilog;
using Akka.Remote.Hosting;
using AkkaExceptionSerializationIssue.Shared;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace AkkaExceptionSerializationIssue.Net8
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Services
                    .AddAkka("akka-exception-serialization-issue", (akkaBuilder, provider) =>
                    {
                        akkaBuilder
                            .ConfigureLoggers(options =>
                            {
                                options.ClearLoggers();
                                options.AddLogger<SerilogLogger>();
                            })
                            .WithRemoting(hostname: "localhost", port: 55455)
                            .WithClustering(new ClusterOptions
                            {
                                SeedNodes = ["akka.tcp://akka-exception-serialization-issue@localhost:55455"],
                            })
                            .WithSingleton<ParentActor>("parent-actor", ParentActor.Props(), createProxyToo: true)
                            .WithActors((system, registry, resolver) =>
                            {
                            });
                    });
                builder.Services
                    .AddSerilog()
                    .AddEndpointsApiExplorer()
                    .AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("v1", new OpenApiInfo
                        {
                            Version = "v1",
                            Title = "Akka Exception Serialization Issue API",
                        });

                        options.CustomOperationIds(x => ((ControllerActionDescriptor)x.ActionDescriptor).ActionName);
                        options.DescribeAllParametersInCamelCase();

                        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                        if (File.Exists(xmlPath))
                        {
                            options.IncludeXmlComments(xmlPath);
                        }
                    });

                builder.Services
                    .AddControllers();

                var app = builder.Build();

                app.UseSerilogRequestLogging();

                app.UseSwagger();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Akka Exception Serialization Issue API v1"));

                app.MapControllers();

                app.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}