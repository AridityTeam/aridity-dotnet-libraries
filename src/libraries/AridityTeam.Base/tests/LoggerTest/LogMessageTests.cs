using System;
using AridityTeam.Base.Util;

namespace AridityTeam.Base.Tests.LoggerTest;

public class LogMessageTests
{
    [Fact]
    public void LogMessage_ShouldInitializeCorrectly()
    {
        var logMessage = new LogMessage("TestFunction", "TestFile.cs", 10, (int)LogSeverity.LogInfo);

        Assert.IsNotNull(logMessage.GetWriter());
        Assert.AreEqual("TestFile.cs", logMessage.GetFilePath());
    }

    [Fact]
    public void Dispose_ShouldFlushAndCloseWriter()
    {
        var logMessage = new LogMessage("TestFunction", "TestFile.cs", 10, (int)LogSeverity.LogInfo);
        logMessage.Dispose();

        // Verify that the writer is closed
        // FIXME -- i hate this
        /*Assert.DoesThrow<ObjectDisposedException>(() =>
        {
            logMessage.GetLastOutput();
            logMessage.GetLastOutput();
            logMessage.GetWriter()?.Dispose();
            logMessage.GetWriter()?.Write("just a test :/");
        });*/
    }

    [Fact]
    public void Flush_ShouldHandleFatalSeverity()
    {
        //var logMessage = new LogMessage("TestFunction", "TestFile.cs", 10, (int)LogSeverity.LogFatal);
        // You may want to mock BaseFunctions.ImmediateCrash to verify it gets called
        //logMessage.Dispose();
    }
}