using AridityTeam.Base.Util;

LoggingSettings loggingSettings = new LoggingSettings()
{
    Destination = LoggingDestination.LogNone
};
var logger = new Logger();
logger.InitLogging(loggingSettings);

logger.Log(LogSeverity.LogInfo, "Starting up...");

HeartbeatInstance heartbeatInstance = new HeartbeatInstance()
{
    InstanceName = "HeartbeatExample",
    ActionToRun = () => { logger.Log(LogSeverity.LogInfo, "Look! This message will be displayed every 5 seconds!"); },
    HeartbeatTime = 5000
};

HeartbeatManager.Instance.AddInstance(heartbeatInstance);

logger.Log(LogSeverity.LogInfo, "Remember: you can press any key to stop...");
Console.ReadKey();
