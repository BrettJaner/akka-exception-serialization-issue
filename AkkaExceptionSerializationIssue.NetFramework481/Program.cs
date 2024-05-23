using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Hosting;
using Akka.Logger.Serilog;
using Akka.Remote.Hosting;
using AkkaExceptionSerializationIssue.Shared;
using Serilog;
using Serilog.Events;

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
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services
                .AddSerilog()
                .AddAkka("akka-exception-serialization-issue", (akkaBuilder, provider) =>
                {
                    akkaBuilder
                        .ConfigureLoggers(options =>
                        {
                            options.ClearLoggers();
                            options.AddLogger<SerilogLogger>();
                        })
                        .WithRemoting(hostname: "localhost", port: 55456)
                        .WithClustering(new ClusterOptions
                        {
                            SeedNodes = ["akka.tcp://akka-exception-serialization-issue@localhost:55455"],
                        })
                        .WithSingletonProxy<ParentActor>("parent-actor")
                        .WithActors((system, registry, resolver) =>
                        {
                            var dispatcher = registry.Get<ParentActor>();

                            var addDevice = new ParentActor.Messages.RegisterChild
                            {
                                ChildName = "child-1",
                            };

                            dispatcher.Tell(addDevice, ActorRefs.Nobody);
                        });
                });

            builder.Build().Run();

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