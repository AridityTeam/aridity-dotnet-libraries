using AridityTeam.Base.Util;

var settings = new LoggingSettings()
{
    Destination = LoggingDestination.LogToFile,
    EnableSentry = true,
    SentryOptions = new SentryOptions()
    {
        Dsn = "https://2b3ea381329c463578ffacd71e8a5806@o4507485361668096.ingest.de.sentry.io/4508856414175312",
    }
};
var lineParser = new CommandLineParser(args);
var logger = new Logger();
if(!logger.InitLogging(settings))
    throw new Exception("Failed to init logging settings");

logger.Log(LogSeverity.LogInfo, "Hello world!");
logger.Log(LogSeverity.LogInfo, "This is the implementation or most likely a port of the Chromium logger in .NET!");
logger.Log(LogSeverity.LogInfo, "This is an info message.");
logger.Log(LogSeverity.LogWarning, "This is an warning message.");
logger.Log(LogSeverity.LogError, "This is an error message.");

if(lineParser.FindParm("/enableFatal"))
    logger.Log(LogSeverity.LogFatal, "This is an fatal message. It's going to crash the program!" );