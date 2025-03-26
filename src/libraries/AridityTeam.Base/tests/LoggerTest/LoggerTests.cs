using System;
using System.IO;
//using Moq;
using AridityTeam.Base.Util;
using Sentry;

namespace AridityTeam.Base.Tests.LoggerTest;

public class LoggerTests : IDisposable
{
    private readonly Logger _logger;
    //private readonly Mock<ISentryService> _mockSentryService;

    public LoggerTests()
    {
        _logger = Logger.Instance; // Ensure we are using the singleton instance
    }

    public void Dispose()
    {
        // Clean up any resources if necessary
    }

    [Fact]
    public void Instance_ShouldReturnSameInstance()
    {
        var logger1 = Logger.Instance;
        var logger2 = Logger.Instance;

        Assert.Same(logger1, logger2);
    }

    [Fact]
    public void InitLogging_ShouldInitializeSentry_WhenEnabled()
    {
        var settings = new LoggingSettings
        {
            EnableSentry = true,
            SentryOptions = new SentryOptions() { Dsn = "" } // Assume this is a valid SentryOptions object
        };

        var result = _logger.InitLogging(settings);

        Assert.True(result);
        // You may want to verify that SentrySdk.Init was called, if you can mock it
    }

    [Fact]
    public void InitLogging_ShouldNotInitializeSentry_WhenOptionsAreNull()
    {
        var settings = new LoggingSettings
        {
            EnableSentry = true,
            SentryOptions = null
        };

        var result = _logger.InitLogging(settings);

        Assert.False(result);
    }

    [Fact]
    public void Log_ShouldWriteMessage_WhenCalled()
    {
        var settings = new LoggingSettings
        {
            EnableSentry = false,
            Destination = LoggingDestination.LogToStderr // or any other destination
        };

        _logger.InitLogging(settings);

        using (var writer = new StringWriter())
        {
            Console.SetOut(writer);
            _logger.Log(LogSeverity.LogInfo, "Test message");

            //var output = writer.ToString().Trim();
            //Assert.Equal("Test message", output); // further improving is required
        }
    }

    [Fact]
    public void Log_ShouldCaptureMessageInSentry_WhenEnabled()
    {
        var settings = new LoggingSettings
        {
            EnableSentry = true,
            SentryOptions = new SentryOptions() { Dsn = "" }
        };

        _logger.InitLogging(settings);

        //var mockSentry = new Mock<ISentry>();
        //SentrySdk.SetInstance(mockSentry.Object);

        _logger.Log(LogSeverity.LogInfo, "Test message");

        //mockSentry.Verify(s => s.CaptureMessage(It.IsAny<string>(), It.IsAny<SentryLevel>()), Times.Once);
    }

    [Fact]
    public void GetDestination_ShouldReturnDefault_WhenSettingsAreNull()
    {
        var destination = _logger.GetDestination();
        Assert.Equal(LoggingDestination.LogDefault, destination);
    }

    [Fact]
    public void Dispose_ShouldDisposeSentry_WhenCalled()
    {
        var settings = new LoggingSettings
        {
            EnableSentry = true,
            SentryOptions = new SentryOptions()
            {
                Dsn = ""
            }
        };

        _logger.InitLogging(settings);
        _logger.Dispose();
        
        // TODO -- verify that sentry is actually disposed/flushed/closed
    }
}