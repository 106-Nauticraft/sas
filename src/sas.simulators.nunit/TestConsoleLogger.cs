using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace sas.simulators.nunit;

/// <summary>
/// This class implements ILogger and will write all logs to output using NUnit's TestContext.WriteLine().
/// This is to help logging in some test scenarios that require better logging than using NSubstitute bluntly.
/// </summary>
public class TestConsoleLogger(TextWriter? outputWriter = null, LogLevel maxLevel = LogLevel.Trace)
    : ILogger
{
    private readonly TextWriter _outputWriter = outputWriter ?? TestContext.Out;

    public TestConsoleLogger() : this(TestContext.Out)
    {
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (logLevel < maxLevel)
        {
            return;
        }

        var logMessage = $"{DateTime.Now:HH:mm:ss.fff} [{logLevel}] {state}";

        if (exception != null)
        {
            logMessage += $"{Environment.NewLine}Exception was : {exception}";
        }

        _outputWriter.WriteLine(logMessage);
    }
    
    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        private NullScope()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }


    #if NET7_0_OR_GREATER
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    #else 
    public IDisposable BeginScope<TState>(TState state)
    #endif
    {
        return NullScope.Instance;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }
}

internal class TestConsoleLogger<T> : TestConsoleLogger, ILogger<T>
{
    public TestConsoleLogger()
    {
    }

    public TestConsoleLogger(TextWriter? outputWriter, LogLevel maxLevel = LogLevel.Trace) : base(outputWriter, maxLevel)
    {
    }
}
