using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

public static class TestLoggerFactory
{
    private static ILoggerFactory _factory;

    public static ILogger<T> CreateLogger<T>()
    {
        if (_factory == null)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341") // Change if your Seq is remote
                .CreateLogger();

            _factory = new LoggerFactory(new[] {
                new SerilogLoggerProvider(Log.Logger, dispose: true)
            });
        }

        return _factory.CreateLogger<T>();
    }
}
