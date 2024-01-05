using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace sas.simulators.nunit;

/// <summary>
/// This class implements ILogger and will write all logs to output using NUnit's TestContext.WriteLine().
/// This is to help logging in some test scenarios that require better logging than using NSubstitute bluntly.
/// </summary>
public class TestConsoleLogger : ILogger
{
    private readonly TextWriter _outputWriter;
    private readonly LogLevel _maxLevel;

    public TestConsoleLogger(TextWriter? outputWriter = null, LogLevel maxLevel = LogLevel.Trace)
    {
        _outputWriter = outputWriter ?? TestContext.Out;
        _maxLevel = maxLevel;
    }

    public TestConsoleLogger() : this(TestContext.Out)
    {
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (logLevel < _maxLevel)
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


#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning disable CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
#pragma warning restore CS8633 // Nullability in constraints for type parameter doesn't match the constraints for type parameter in implicitly implemented interface method'.
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
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
